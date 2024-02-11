using System.Diagnostics;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;

namespace EntityFrameworkCore.Triggered.Transactions.Internal;

public class BeforeRollbackTriggerDescriptor : ITriggerTypeDescriptor
{
    readonly Action<object, object> _invocationDelegate;
    readonly Type _triggerType;

    public BeforeRollbackTriggerDescriptor(Type entityType)
    {
        var triggerType = typeof(IBeforeRollbackTrigger<>).MakeGenericType(entityType);
        var triggerMethod = triggerType.GetMethod(nameof(IBeforeRollbackTrigger<object>.BeforeRollback));

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
