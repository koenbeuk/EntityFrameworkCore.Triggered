using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

        public Task Invoke(object triggerContext, CancellationToken cancellationToken)
            => _triggerTypeDescriptor.Invoke(_trigger, triggerContext, cancellationToken);
        
    }
}
