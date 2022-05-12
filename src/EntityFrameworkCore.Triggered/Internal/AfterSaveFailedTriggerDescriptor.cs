using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class AfterSaveFailedTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Func<object, object, Exception?, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public AfterSaveFailedTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IAfterSaveFailedTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IAfterSaveFailedTrigger<object>.AfterSaveFailed));

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetWeakDelegateWithException(triggerType, entityType, triggerMethod!);
        }

        public Type TriggerType => _triggerType;

        public Task Invoke(object trigger, object triggerContext, Exception? exception, CancellationToken cancellationToken)
            => _invocationDelegate(trigger, triggerContext, exception, cancellationToken);

    }
}
