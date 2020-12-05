using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal.CascadingStrategies
{
    public interface ICascadingStrategy
    {
        bool CanCascade(EntityEntry entry, ChangeType changeType, TriggerContextDescriptor previousTriggerContextDescriptor);
    }
}
