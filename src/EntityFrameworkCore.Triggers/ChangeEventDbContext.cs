using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggers
{
    public abstract class ChangeEventDbContext : DbContext
    {
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var changeEventService = this.GetService<IChangeEventService>() ?? throw new InvalidOperationException("Change events are not configured");

            var changeEventSession = changeEventService.CreateSession(this);

            changeEventSession.RaiseBeforeSaveChangeEvents(default).GetAwaiter().GetResult();
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            changeEventSession.RaiseAfterSaveChangeEvents(default).GetAwaiter().GetResult();

            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var changeEventService = this.GetService<IChangeEventService>() ?? throw new InvalidOperationException("Change events are not configured");

            var changeEventSession = changeEventService.CreateSession(this);

            await changeEventSession.RaiseBeforeSaveChangeEvents(default).ConfigureAwait(false);
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            await changeEventSession.RaiseAfterSaveChangeEvents(default).ConfigureAwait(false);

            return result;
        }
    }
}
