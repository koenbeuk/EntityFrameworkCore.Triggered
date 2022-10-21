using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal.Descriptors
{
    public sealed class AfterSaveFailedTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Action<object, object, Exception?> _invocationDelegate;
        readonly Type _triggerType;

        public AfterSaveFailedTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IAfterSaveFailedTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IAfterSaveFailedTrigger<object>.AfterSaveFailed));

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetWeakDelegateWithException(triggerType, entityType, triggerMethod!);
        }

        public Type TriggerType => _triggerType;

        public void Invoke(object trigger, object triggerContext, Exception? exception)
            => _invocationDelegate(trigger, triggerContext, exception);
    }
}
