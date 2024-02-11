using System.Diagnostics;

namespace EntityFrameworkCore.Triggered.Internal.Descriptors
{
    public sealed class AfterSaveTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Action<object, object> _invocationDelegate;
        readonly Type _triggerType;

        public AfterSaveTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IAfterSaveTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IAfterSaveTrigger<object>.AfterSave));

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetWeakDelegate(triggerType, entityType, triggerMethod!);
        }

        public Type TriggerType => _triggerType;

        public void Invoke(object trigger, object triggerContext, Exception? exception)
        {
            Debug.Assert(exception == null);

            _invocationDelegate(trigger, triggerContext);
        }
    }
}
