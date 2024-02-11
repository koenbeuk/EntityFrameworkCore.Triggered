using System.Diagnostics;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;

namespace EntityFrameworkCore.Triggered.Transactions.Internal
{
    public class BeforeCommitAsyncTriggerDescriptor : IAsyncTriggerTypeDescriptor
    {
        readonly Func<object, object, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public BeforeCommitAsyncTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IBeforeCommitAsyncTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IBeforeCommitAsyncTrigger<object>.BeforeCommitAsync));

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
