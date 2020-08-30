using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal.RecursionStrategy
{
    public class EntityAndTypeRecursionStrategy : IRecursionStrategy
    {
        public bool CanRecurse(EntityEntry entry, ChangeType changeType, TriggerContextDescriptor previousTriggerContextDescriptor)
            => changeType != previousTriggerContextDescriptor.ChangeType;
    }
}
