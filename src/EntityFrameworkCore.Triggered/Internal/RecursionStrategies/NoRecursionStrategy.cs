using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal.RecursionStrategy
{
    public class NoRecursionStrategy : IRecursionStrategy
    {
        public bool CanRecurse(EntityEntry entry, ChangeType changeType, TriggerContextDescriptor previousTriggerContextDescriptor)
            => false;
    }
}
