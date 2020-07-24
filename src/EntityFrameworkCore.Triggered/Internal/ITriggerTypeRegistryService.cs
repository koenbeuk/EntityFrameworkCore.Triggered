using System;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerTypeRegistryService
    {
        TriggerTypeRegistry ResolveRegistry(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory);
    }
}