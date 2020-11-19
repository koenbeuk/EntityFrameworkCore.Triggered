using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
