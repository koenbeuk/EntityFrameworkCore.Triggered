using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal.RecursionStrategy
{
    public interface IRecursionStrategy
    {
        bool CanRecurse(EntityEntry entry, ChangeType changeType, TriggerContextDescriptor previousTriggerContextDescriptor);
    }
}
