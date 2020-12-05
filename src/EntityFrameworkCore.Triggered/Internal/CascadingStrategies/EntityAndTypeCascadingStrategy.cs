using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal.CascadingStrategies
{
    public class EntityAndTypeCascadingStrategy : ICascadingStrategy
    {
        public bool CanCascade(EntityEntry entry, ChangeType changeType, TriggerContextDescriptor previousTriggerContextDescriptor)
            => changeType != previousTriggerContextDescriptor.ChangeType;
    }
}
