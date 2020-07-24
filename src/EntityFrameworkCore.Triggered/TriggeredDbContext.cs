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
        protected TriggeredDbContext()
            : this(new DbContextOptions<DbContext>())
        {
        }

        protected TriggeredDbContext(DbContextOptions options)
            : base(options)
        {
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
            var triggerService = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");

            var triggerSession = triggerService.CreateSession(this);

            triggerSession.RaiseBeforeSaveTriggers(default).GetAwaiter().GetResult();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            triggerSession.RaiseAfterSaveTriggers(default).GetAwaiter().GetResult();

            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var triggerService = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");

            var triggerSession = triggerService.CreateSession(this);

            await triggerSession.RaiseBeforeSaveTriggers(cancellationToken).ConfigureAwait(false);
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            await triggerSession.RaiseAfterSaveTriggers(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
