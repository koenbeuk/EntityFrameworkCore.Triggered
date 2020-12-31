using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal.CascadeStrategies
{
    public class EntityAndTypeCascadeStrategy : ICascadeStrategy
    {
        public bool CanCascade(EntityEntry entry, ChangeType changeType, TriggerContextDescriptor previousTriggerContextDescriptor)
            => changeType != previousTriggerContextDescriptor.ChangeType;
    }
}
