using EntityFrameworkCore.Triggered;

namespace BlazorTests.Triggers.Counts;

public class PublishCountAddedEvent(EventAggregator eventAggregator) : IAfterSaveTrigger<Count>
{
    private readonly EventAggregator _eventAggregator = eventAggregator;

    public void AfterSave(ITriggerContext<Count> context)
    {
        if (context.ChangeType == ChangeType.Added)
        {
            _eventAggregator.PublishCountAdded(context.Entity);
        }
    }
}
