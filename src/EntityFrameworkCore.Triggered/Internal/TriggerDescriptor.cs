using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal
{
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
        }

        public ITriggerTypeDescriptor TypeDescriptor => _triggerTypeDescriptor;
        public object Trigger => _trigger;
        public int Priority => _priority;

        public Task Invoke(object triggerContext, Exception? exception, CancellationToken cancellationToken)
            => _triggerTypeDescriptor.Invoke(_trigger, triggerContext, exception, cancellationToken);

    }
}
