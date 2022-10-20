using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal.Descriptors
{
    public sealed class AfterSaveFailedAsyncTriggerDescriptor : IAsyncTriggerTypeDescriptor
    {
        readonly Func<object, object, Exception?, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public AfterSaveFailedAsyncTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IAfterSaveFailedAsyncTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IAfterSaveFailedAsyncTrigger<object>.AfterSaveFailedAsync));

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetAsyncWeakDelegateWithException(triggerType, entityType, triggerMethod!);
        }

        public Type TriggerType => _triggerType;

        public Task Invoke(object trigger, object triggerContext, Exception? exception, CancellationToken cancellationToken)
            => _invocationDelegate(trigger, triggerContext, exception, cancellationToken);

    }
}
