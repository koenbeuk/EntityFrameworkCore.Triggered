using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class BeforeSaveTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Func<object, object, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public BeforeSaveTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IBeforeSaveTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IBeforeSaveTrigger<object>.BeforeSave));

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetWeakDelegate(triggerType, entityType, triggerMethod!);
        }

        public Type TriggerType => _triggerType;

        public Task Invoke(object trigger, object triggerContext, Exception? exception, CancellationToken cancellationToken)
        {
            Debug.Assert(exception == null);

            return _invocationDelegate(trigger, triggerContext, cancellationToken);
        }

    }
}
