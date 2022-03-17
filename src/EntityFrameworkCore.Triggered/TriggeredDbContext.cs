using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered
{

    [Obsolete("With the release of EntityFrameworkCore 5 and SaveChangesInterceptor, we no longer need to derive our DbContext from TriggeredDbContext")]
    public abstract class TriggeredDbContext : DbContext
    {
        IServiceProvider? _triggerServiceProvider;
        ITriggerSession? _triggerSession;

        protected TriggeredDbContext()
            : this(new DbContextOptions<DbContext>(), null)
        {
        }

        protected TriggeredDbContext(DbContextOptions options)
            : this(options, null)
        {
        }

        protected TriggeredDbContext(IServiceProvider? serviceProvider)
            : this(new DbContextOptions<DbContext>(), serviceProvider)
        {
        }

        protected TriggeredDbContext(DbContextOptions options, IServiceProvider? serviceProvider)
            : base(options)
        {
            _triggerServiceProvider = serviceProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseTriggers();
            }

            base.OnConfiguring(optionsBuilder);
        }

        public void SetTriggerServiceProvider(IServiceProvider? serviceProvider)
            => _triggerServiceProvider = serviceProvider;
        
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            bool RaiseAfterSavFailedTriggers(Exception exception)
            {
                _triggerSession.RaiseAfterSaveFailedStartingTriggers(exception).GetAwaiter().GetResult();
                _triggerSession.RaiseAfterSaveFailedTriggers(exception).GetAwaiter().GetResult();
                _triggerSession.RaiseAfterSaveFailedCompletedTriggers(exception).GetAwaiter().GetResult();

                return false;
            }

            var createdTriggerSession = false;

            if (_triggerSession is null)
            {
                var triggerService = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");

                if (triggerService.Current is not null)
                {
                    _triggerSession = triggerService.Current;
                }
                else
                {
                    _triggerSession = triggerService.CreateSession(this, _triggerServiceProvider);
                    createdTriggerSession = true;
                }
            }

            try
            {
                int result;
                var defaultAutoDetectChangesEnabled = ChangeTracker.AutoDetectChangesEnabled;

                try
                {
                    ChangeTracker.AutoDetectChangesEnabled = false;

                    _triggerSession.RaiseBeforeSaveStartingTriggers().GetAwaiter().GetResult();
                    _triggerSession.RaiseBeforeSaveTriggers().GetAwaiter().GetResult();
                    _triggerSession.CaptureDiscoveredChanges();
                    _triggerSession.RaiseBeforeSaveCompletedTriggers().GetAwaiter().GetResult();

                    try
                    {
                        result = base.SaveChanges(acceptAllChangesOnSuccess);
                    }
                    catch (Exception exception) when (RaiseAfterSavFailedTriggers(exception))
                    {
                        throw; // Should never reach
                    }
                }
                finally
                {
                    ChangeTracker.AutoDetectChangesEnabled = defaultAutoDetectChangesEnabled;
                }

                _triggerSession.RaiseAfterSaveStartingTriggers().GetAwaiter().GetResult();
                _triggerSession.RaiseAfterSaveTriggers().GetAwaiter().GetResult();
                _triggerSession.RaiseAfterSaveCompletedTriggers().GetAwaiter().GetResult();

                return result;
            }
            finally
            {
                if (createdTriggerSession)
                {
                    _triggerSession.Dispose();
                    _triggerSession = null;
                }
            }
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            async Task RaiseAfterSaveFailedTriggers(Exception exception, CancellationToken cancellationToken)
            {
                await _triggerSession.RaiseAfterSaveFailedStartingTriggers(exception, cancellationToken);
                await _triggerSession.RaiseAfterSaveFailedTriggers(exception, cancellationToken);
                await _triggerSession.RaiseAfterSaveFailedCompletedTriggers(exception, cancellationToken);
            }

            var createdTriggerSession = false;

            if (_triggerSession == null)
            {
                var triggerService = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");

                if (triggerService.Current != null)
                {
                    _triggerSession = triggerService.Current;
                }
                else
                {
                    _triggerSession = triggerService.CreateSession(this, _triggerServiceProvider);
                    createdTriggerSession = true;
                }
            }
            try
            {

                int result;
                var defaultAutoDetectChangesEnabled = ChangeTracker.AutoDetectChangesEnabled;

                try
                {
                    ChangeTracker.AutoDetectChangesEnabled = false;

                    await _triggerSession.RaiseBeforeSaveStartingTriggers(cancellationToken).ConfigureAwait(false);
                    await _triggerSession.RaiseBeforeSaveTriggers(cancellationToken).ConfigureAwait(false);
                    _triggerSession.CaptureDiscoveredChanges();
                    await _triggerSession.RaiseBeforeSaveCompletedTriggers(cancellationToken).ConfigureAwait(false);

                    try
                    {
                        result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        await RaiseAfterSaveFailedTriggers(exception, cancellationToken);
                        throw;
                    }
                }
                finally
                {
                    ChangeTracker.AutoDetectChangesEnabled = defaultAutoDetectChangesEnabled;
                }

                await _triggerSession.RaiseAfterSaveStartingTriggers(cancellationToken).ConfigureAwait(false);
                await _triggerSession.RaiseAfterSaveTriggers(cancellationToken).ConfigureAwait(false);
                await _triggerSession.RaiseAfterSaveCompletedTriggers(cancellationToken).ConfigureAwait(false);

                return result;
            }
            finally
            {
                if (createdTriggerSession)
                {
                    _triggerSession.Dispose();
                    _triggerSession = null;
                }
            }
        }
    }
}
