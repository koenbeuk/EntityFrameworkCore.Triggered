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
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var Triggerservice = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");

            var Triggersession = Triggerservice.CreateSession(this);

            Triggersession.RaiseBeforeSaveTriggers(default).GetAwaiter().GetResult();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            Triggersession.RaiseAfterSaveTriggers(default).GetAwaiter().GetResult();

            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var Triggerservice = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");

            var Triggersession = Triggerservice.CreateSession(this);

            await Triggersession.RaiseBeforeSaveTriggers(default).ConfigureAwait(false);
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            await Triggersession.RaiseAfterSaveTriggers(default).ConfigureAwait(false);

            return result;
        }
    }
}
