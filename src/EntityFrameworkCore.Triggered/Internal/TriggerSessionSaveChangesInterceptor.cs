using System.Diagnostics;
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
            EnlistTriggerSession(eventData);
            Debug.Assert(_triggerSession != null);

            var defaultAutoDetectChangesEnabled = eventData.Context!.ChangeTracker.AutoDetectChangesEnabled;

            try
            {
                eventData.Context.ChangeTracker.AutoDetectChangesEnabled = false;

                _triggerSession.RaiseBeforeSaveStartingTriggers();
                _triggerSession.RaiseBeforeSaveTriggers();
                _triggerSession.CaptureDiscoveredChanges();
                _triggerSession.RaiseBeforeSaveCompletedTriggers();
            }
            catch
            {
                // We're aborting the SaveChanges call, delist the trigger session now
                DelistTriggerSession(eventData);
                throw;
            }
            finally
            {
                eventData.Context.ChangeTracker.AutoDetectChangesEnabled = defaultAutoDetectChangesEnabled;
            }

            return result;
        }

        public async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            EnlistTriggerSession(eventData);
            Debug.Assert(_triggerSession != null);

            var defaultAutoDetectChangesEnabled = eventData.Context!.ChangeTracker.AutoDetectChangesEnabled;

            try
            {
                eventData.Context.ChangeTracker.AutoDetectChangesEnabled = false;

                _triggerSession.RaiseBeforeSaveStartingTriggers();
                await _triggerSession.RaiseBeforeSaveStartingAsyncTriggers(cancellationToken).ConfigureAwait(false);

                _triggerSession.RaiseBeforeSaveTriggers();
                await _triggerSession.RaiseBeforeSaveAsyncTriggers(cancellationToken).ConfigureAwait(false);

                _triggerSession.CaptureDiscoveredChanges();

                _triggerSession.RaiseBeforeSaveCompletedTriggers();
                await _triggerSession.RaiseBeforeSaveCompletedAsyncTriggers(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // We're aborting the SaveChanges call, delist the trigger session now
                DelistTriggerSession(eventData);
                throw;
            }
            finally
            {
                eventData.Context.ChangeTracker.AutoDetectChangesEnabled = defaultAutoDetectChangesEnabled;
            }

            return result;
        }

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            Debug.Assert(_triggerSession != null);

            _triggerSession.RaiseAfterSaveStartingTriggers();
            _triggerSession.RaiseAfterSaveTriggers();
            _triggerSession.RaiseAfterSaveCompletedTriggers();

            DelistTriggerSession(eventData);

            return result;
        }

        public async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            Debug.Assert(_triggerSession != null);

            _triggerSession.RaiseAfterSaveStartingTriggers();
            await _triggerSession.RaiseAfterSaveStartingAsyncTriggers(cancellationToken).ConfigureAwait(false);

            _triggerSession.RaiseAfterSaveTriggers();
            await _triggerSession.RaiseAfterSaveAsyncTriggers(cancellationToken).ConfigureAwait(false);

            _triggerSession.RaiseAfterSaveCompletedTriggers();
            await _triggerSession.RaiseAfterSaveCompletedAsyncTriggers(cancellationToken).ConfigureAwait(false);

            DelistTriggerSession(eventData);

            return result;
        }

        public void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            Debug.Assert(_triggerSession != null);

            _triggerSession.RaiseAfterSaveFailedStartingTriggers(eventData.Exception);
            _triggerSession.RaiseAfterSaveFailedTriggers(eventData.Exception);
            _triggerSession.RaiseAfterSaveFailedCompletedTriggers(eventData.Exception);

            DelistTriggerSession(eventData);
        }

        public async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
        {
            Debug.Assert(_triggerSession != null);

            _triggerSession.RaiseAfterSaveFailedStartingTriggers(eventData.Exception);
            await _triggerSession.RaiseAfterSaveFailedStartingAsyncTriggers(eventData.Exception, cancellationToken).ConfigureAwait(false);

            _triggerSession.RaiseAfterSaveFailedTriggers(eventData.Exception);
            await _triggerSession.RaiseAfterSaveFailedAsyncTriggers(eventData.Exception, cancellationToken).ConfigureAwait(false);

            _triggerSession.RaiseAfterSaveFailedCompletedTriggers(eventData.Exception);
            await _triggerSession.RaiseAfterSaveFailedCompletedAsyncTriggers(eventData.Exception, cancellationToken).ConfigureAwait(false);

            DelistTriggerSession(eventData);
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete
}
