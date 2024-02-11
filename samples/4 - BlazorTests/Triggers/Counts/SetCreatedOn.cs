using System;
using EntityFrameworkCore.Triggered;

namespace BlazorTests.Triggers.Counts
{
    public class SetCreatedOn : IBeforeSaveTrigger<Count>
    {
        public void BeforeSave(ITriggerContext<Count> context) => context.Entity.CreatedOn = DateTime.UtcNow;
    }
}
