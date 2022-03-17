using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered.Internal
{
#pragma warning disable CS0618 // Type or member is obsolete (TriggeredDbContext with EFCore5)
    public class TriggerSessionSaveChangesInterceptor : ISaveChangesInterceptor
    {
#if DEBUG
        DbContext? _capturedDbContext;
#endif

        ITriggerSession? _triggerSession;
        int _parallelSaveChangesCount;

        private void EnlistTriggerSession(DbContextEventData eventData)
        {
#if DEBUG
            if (_triggerSession != null)
            {
                Debug.Assert(_capturedDbContext == eventData.Context);
            }
            else
            {
                _capturedDbContext = eventData.Context;
            }
#endif

            if (_triggerSession == null)
            {
                if (eventData.Context is null)
                {
                    throw new InvalidOperationException("Expected a context");
                }

                var triggerService = eventData.Context.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");

                if (triggerService.Current != null)
                {
                    _triggerSession = triggerService.Current;
                }
                else
                {
                    _triggerSession = triggerService.CreateSession(eventData.Context);
                }
            }

            _parallelSaveChangesCount += 1;
        }

        private void DelistTriggerSession(DbContextEventData eventData)
        {
            Debug.Assert(_triggerSession != null);

#if DEBUG
            Debug.Assert(_capturedDbContext == eventData.Context);
#endif

            _parallelSaveChangesCount -= 1;

            if (_parallelSaveChangesCount == 0)
            {
                _triggerSession.Dispose();
                _triggerSession = null;
            }
        }


        public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (!(eventData.Context is TriggeredDbContext))
            {
                EnlistTriggerSession(eventData);
                Debug.Assert(_triggerSession != null);

                var defaultAutoDetectChangesEnabled = eventData.Context!.ChangeTracker.AutoDetectChangesEnabled;

                try
                {

                    eventData.Context.ChangeTracker.AutoDetectChangesEnabled = false;

                    _triggerSession.RaiseBeforeSaveStartingTriggers().GetAwaiter().GetResult();
                    _triggerSession.RaiseBeforeSaveTriggers().GetAwaiter().GetResult();
                    _triggerSession.CaptureDiscoveredChanges();
                    _triggerSession.RaiseBeforeSaveCompletedTriggers().GetAwaiter().GetResult();
                }
                finally
                {
                    eventData.Context.ChangeTracker.AutoDetectChangesEnabled = defaultAutoDetectChangesEnabled;
                }

                return result;
            }

            return result;
        }

        public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (!(eventData.Context is TriggeredDbContext))
            {
                EnlistTriggerSession(eventData);
                Debug.Assert(_triggerSession != null);

                var defaultAutoDetectChangesEnabled = eventData.Context!.ChangeTracker.AutoDetectChangesEnabled;

                try
                {
                    eventData.Context.ChangeTracker.AutoDetectChangesEnabled = false;

                    await _triggerSession.RaiseBeforeSaveStartingTriggers(cancellationToken).ConfigureAwait(false);
                    await _triggerSession.RaiseBeforeSaveTriggers(cancellationToken).ConfigureAwait(false);
                    _triggerSession.CaptureDiscoveredChanges();
                    await _triggerSession.RaiseBeforeSaveCompletedTriggers(cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    eventData.Context.ChangeTracker.AutoDetectChangesEnabled = defaultAutoDetectChangesEnabled;
                }
            }

            return result;
        }

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            if (!(eventData.Context is TriggeredDbContext))
            {
                Debug.Assert(_triggerSession != null);

                _triggerSession.RaiseAfterSaveStartingTriggers().GetAwaiter().GetResult();
                _triggerSession.RaiseAfterSaveTriggers().GetAwaiter().GetResult();
                _triggerSession.RaiseAfterSaveCompletedTriggers().GetAwaiter().GetResult();

                DelistTriggerSession(eventData);
            }

            return result;
        }

        public async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            if (!(eventData.Context is TriggeredDbContext))
            {
                Debug.Assert(_triggerSession != null);

                await _triggerSession.RaiseAfterSaveStartingTriggers(cancellationToken).ConfigureAwait(false);
                await _triggerSession.RaiseAfterSaveTriggers(cancellationToken).ConfigureAwait(false);
                await _triggerSession.RaiseAfterSaveCompletedTriggers(cancellationToken).ConfigureAwait(false);

                DelistTriggerSession(eventData);
            }

            return result;
        }

        public void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            if (!(eventData.Context is TriggeredDbContext))
            {
                Debug.Assert(_triggerSession != null);

                _triggerSession.RaiseAfterSaveFailedStartingTriggers(eventData.Exception).GetAwaiter().GetResult();
                _triggerSession.RaiseAfterSaveFailedTriggers(eventData.Exception).GetAwaiter().GetResult();
                _triggerSession.RaiseAfterSaveFailedCompletedTriggers(eventData.Exception).GetAwaiter().GetResult();

                DelistTriggerSession(eventData);
            }
        }

        public async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            if (!(eventData.Context is TriggeredDbContext))
            {
                Debug.Assert(_triggerSession != null);

                await _triggerSession.RaiseAfterSaveFailedStartingTriggers(eventData.Exception, cancellationToken).ConfigureAwait(false);
                await _triggerSession.RaiseAfterSaveFailedTriggers(eventData.Exception, cancellationToken).ConfigureAwait(false);
                await _triggerSession.RaiseAfterSaveFailedCompletedTriggers(eventData.Exception, cancellationToken).ConfigureAwait(false);

                DelistTriggerSession(eventData);
            }
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
