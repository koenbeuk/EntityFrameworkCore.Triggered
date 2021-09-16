using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;

namespace EntityFrameworkCore.Triggered.Transactions.Internal
{
    public class BeforeRollbackTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Func<object, object, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public BeforeRollbackTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IBeforeRollbackTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IBeforeRollbackTrigger<object>.BeforeRollback));
            Debug.Assert(triggerMethod is not null);

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetWeakDelegate(triggerType, entityType, triggerMethod);
        }

        public Type TriggerType => _triggerType;

        public Task Invoke(object trigger, object triggerContext, Exception? exception, CancellationToken cancellationToken)
        {
            Debug.Assert(exception == null);

            return _invocationDelegate(trigger, triggerContext, cancellationToken);
        }
    }
}
