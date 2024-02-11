using System;
using System.Diagnostics;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;

namespace EntityFrameworkCore.Triggered.Transactions.Internal
{
    public class AfterCommitTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Action<object, object> _invocationDelegate;
        readonly Type _triggerType;

        public AfterCommitTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IAfterCommitTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IAfterCommitTrigger<object>.AfterCommit));

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
