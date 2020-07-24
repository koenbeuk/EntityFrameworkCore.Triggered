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
    public class BeforeRollbackTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Func<object, object, CancellationToken, Task> _invocationDelegate;
        readonly Type _triggerType;

        public BeforeRollbackTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IBeforeRollbackTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IBeforeRollbackTrigger<object>.BeforeRollback));

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetWeakDelegate(triggerType, entityType, triggerMethod);
        }

        public Type TriggerType => _triggerType;

        public Task Invoke(object trigger, object triggerContext, CancellationToken cancellationToken)
            => _invocationDelegate(trigger, triggerContext, cancellationToken);

    }
}
