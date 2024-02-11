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

        public void AfterSave(ITriggerContext<Count> context)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                _eventAggregator.PublishCountAdded(context.Entity);
            }
        }
    }
}
