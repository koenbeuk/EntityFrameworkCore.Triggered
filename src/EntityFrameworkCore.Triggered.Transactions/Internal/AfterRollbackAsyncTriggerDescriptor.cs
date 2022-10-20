using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;

namespace EntityFrameworkCore.Triggered.Transactions.Internal
{
    public class AfterRollbackAsyncTriggerDescriptor : IAsyncTriggerTypeDescriptor
    {
        readonly Func<object, object, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public AfterRollbackAsyncTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IAfterRollbackAsyncTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IAfterRollbackAsyncTrigger<object>.AfterRollbackAsync));

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
