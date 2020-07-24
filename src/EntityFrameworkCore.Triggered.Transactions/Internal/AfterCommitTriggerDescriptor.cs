using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;

namespace EntityFrameworkCore.Triggered.Transactions.Internal
{
    public class AfterCommitTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Func<object, object, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public AfterCommitTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IAfterCommitTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IAfterCommitTrigger<object>.AfterCommit));

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetWeakDelegate(triggerType, entityType, triggerMethod);
        }

        public Type TriggerType => _triggerType;

        public Task Invoke(object trigger, object triggerContext, CancellationToken cancellationToken)
            => _invocationDelegate(trigger, triggerContext, cancellationToken);

    }
}
