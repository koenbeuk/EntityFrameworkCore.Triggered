using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;

namespace EntityFrameworkCore.Triggered.Transactions.Internal
{
    public class BeforeCommitTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Func<object, object, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public BeforeCommitTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IBeforeCommitTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IBeforeCommitTrigger<object>.BeforeCommit));
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
