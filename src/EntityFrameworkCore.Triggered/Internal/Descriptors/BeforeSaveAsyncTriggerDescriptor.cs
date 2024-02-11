using System.Diagnostics;

namespace EntityFrameworkCore.Triggered.Internal.Descriptors
{
    public sealed class BeforeSaveAsyncTriggerDescriptor : IAsyncTriggerTypeDescriptor
    {
        readonly Func<object, object, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public BeforeSaveAsyncTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IBeforeSaveAsyncTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IBeforeSaveAsyncTrigger<object>.BeforeSaveAsync));

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetAsyncWeakDelegate(triggerType, entityType, triggerMethod!);
        }

        public Type TriggerType => _triggerType;

        public Task Invoke(object trigger, object triggerContext, Exception? exception, CancellationToken cancellationToken)
        {
            Debug.Assert(exception == null);

            return _invocationDelegate(trigger, triggerContext, cancellationToken);
        }
    }
}
