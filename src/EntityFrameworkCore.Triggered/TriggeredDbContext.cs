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
            if (_triggerSession == null)
            {
                var triggerService = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");
                _triggerSession = triggerService.CreateSession(this, _triggerServiceProvider);
            }

            try
            {
                _triggerSession.RaiseBeforeSaveTriggers(default).GetAwaiter().GetResult();
                var result = base.SaveChanges(acceptAllChangesOnSuccess);
                _triggerSession.RaiseAfterSaveTriggers(default).GetAwaiter().GetResult();

                return result;
            }
            finally
            {
                _triggerSession = null;
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

                await _triggerSession.RaiseBeforeSaveTriggers(cancellationToken).ConfigureAwait(false);
                var result = base.SaveChanges(acceptAllChangesOnSuccess);
                await _triggerSession.RaiseAfterSaveTriggers(cancellationToken).ConfigureAwait(false);

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
