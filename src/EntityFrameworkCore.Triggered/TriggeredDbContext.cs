using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered
{

#if EFCORETRIGGERED2
    [Obsolete("With the release of EntityFrameworkCore 5 and SaveChangesInterceptor, we no longer need to derive our DbContext from TriggeredDbContext")]
#endif
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
                _triggerSession.RaiseAfterSaveFailedTriggers(exception, default).GetAwaiter().GetResult();

                return false;
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

                    _triggerSession.RaiseBeforeSaveTriggers(default).GetAwaiter().GetResult();
                    _triggerSession.CaptureDiscoveredChanges();

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

                _triggerSession.RaiseAfterSaveTriggers(default).GetAwaiter().GetResult();

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
            Task RaiseAfterSavFailedTriggers(Exception exception, CancellationToken cancellationToken)
            {
                return _triggerSession.RaiseAfterSaveFailedTriggers(exception, cancellationToken);
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

                    await _triggerSession.RaiseBeforeSaveTriggers(default).ConfigureAwait(false);
                    _triggerSession.CaptureDiscoveredChanges();

                    try
                    {
                        result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        await RaiseAfterSavFailedTriggers(exception, cancellationToken);
                        throw;
                    }
                }
                finally
                {
                    ChangeTracker.AutoDetectChangesEnabled = defaultAutoDetectChangesEnabled;
                }

                await _triggerSession.RaiseAfterSaveTriggers(default).ConfigureAwait(false);

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
