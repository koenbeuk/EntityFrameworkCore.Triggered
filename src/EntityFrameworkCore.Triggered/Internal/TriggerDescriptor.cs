using EntityFrameworkCore.Triggered.Internal.Descriptors;

namespace EntityFrameworkCore.Triggered.Internal;

public class TriggerDescriptor
{
    readonly ITriggerTypeDescriptor _triggerTypeDescriptor;
    readonly object _trigger;
    readonly int _priority;

    public TriggerDescriptor(ITriggerTypeDescriptor triggerTypeDescriptor, object trigger)
    {
        _triggerTypeDescriptor = triggerTypeDescriptor ?? throw new ArgumentNullException(nameof(triggerTypeDescriptor));
        _trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));

        if (_trigger is ITriggerPriority triggerPriority)
        {
            _priority = triggerPriority.Priority;
        }
        else
        {
            _priority = 0;
        }
    }

    public ITriggerTypeDescriptor TypeDescriptor => _triggerTypeDescriptor;
    public object Trigger => _trigger;
    public int Priority => _priority;

    public void Invoke(object triggerContext, Exception? exception)
        => _triggerTypeDescriptor.Invoke(_trigger, triggerContext, exception);

}
