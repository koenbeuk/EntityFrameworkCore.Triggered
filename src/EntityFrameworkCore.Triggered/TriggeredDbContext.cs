using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered
{
    public abstract class TriggeredDbContext : DbContext
    {
        readonly IServiceProvider? _triggerServiceProvider;
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

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            bool createdTriggerSession = false;

            if (_triggerSession == null)
            {
                var triggerService = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");
                _triggerSession = triggerService.CreateSession(this, _triggerServiceProvider);
                createdTriggerSession = true;
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
                    result = base.SaveChanges(acceptAllChangesOnSuccess);
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
                    _triggerSession = null;
                }
            }
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            bool createdTriggerSession = false;

            if (_triggerSession == null)
            {
                var triggerService = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");
                _triggerSession = triggerService.CreateSession(this);
                createdTriggerSession = true;
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
                    result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
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
                    _triggerSession = null;
                }
            }
        }
    }
}
