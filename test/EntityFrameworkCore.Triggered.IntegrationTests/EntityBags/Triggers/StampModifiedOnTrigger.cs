using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.IntegrationTests.EntityBags.Triggers
{
    public class StampModifiedOnTrigger : IBeforeSaveTrigger<User>
    {
        public Task BeforeSave(ITriggerContext<User> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType is ChangeType.Modified)
            {
                if (!context.EntityBag.ContainsKey(SoftDeleteTrigger.IsSoftDeleted))
                {
                    context.Entity.ModifiedOn = DateTime.UtcNow;
                }
            }

            return Task.CompletedTask;
        }
    }
}
