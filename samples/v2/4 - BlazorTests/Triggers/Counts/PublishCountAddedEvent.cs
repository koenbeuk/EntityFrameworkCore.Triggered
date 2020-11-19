using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace BlazorTests.Triggers.Counts
{
    public class PublishCountAddedEvent : IAfterSaveTrigger<Count>
    {
        private readonly EventAggregator _eventAggregator;

        public PublishCountAddedEvent(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public Task AfterSave(ITriggerContext<Count> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                _eventAggregator.PublishCountAdded(context.Entity);
            }

            return Task.CompletedTask;
        }
    }
}
