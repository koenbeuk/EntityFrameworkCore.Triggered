using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;

namespace EntityFrameworkCore.Triggered.Transactions.Internal
{
    public class BeforeCommitTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Action<object, object> _invocationDelegate;
        readonly Type _triggerType;

        public BeforeCommitTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IBeforeCommitTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IBeforeCommitTrigger<object>.BeforeCommit));

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
