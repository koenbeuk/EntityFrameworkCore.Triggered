using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal.CascadeStrategies
{
    public class NoCascadeStrategy : ICascadeStrategy
    {
        public bool CanCascade(EntityEntry entry, ChangeType changeType, TriggerContextDescriptor previousTriggerContextDescriptor)
            => false;
    }
}
