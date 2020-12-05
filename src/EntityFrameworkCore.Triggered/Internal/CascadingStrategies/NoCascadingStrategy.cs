using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal.CascadingStrategies
{
    public class NoCascadingStrategy : ICascadingStrategy
    {
        public bool CanCascade(EntityEntry entry, ChangeType changeType, TriggerContextDescriptor previousTriggerContextDescriptor)
            => false;
    }
}
