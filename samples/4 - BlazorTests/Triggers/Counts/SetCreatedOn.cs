using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace BlazorTests.Triggers.Counts
{
    public class SetCreatedOn : IBeforeSaveTrigger<Count>
    {
        public Task BeforeSave(ITriggerContext<Count> context, CancellationToken cancellationToken)
        {
            context.Entity.CreatedOn = DateTime.UtcNow;

            return Task.CompletedTask;
        }
    }
}
