namespace EntityFrameworkCore.Triggered.Internal;

public interface ITriggerTypeRegistryService
{
    TriggerTypeRegistry<TTriggerTypeDescriptor> ResolveRegistry<TTriggerTypeDescriptor>(Type openTriggerType, Type entityType, Func<Type, TTriggerTypeDescriptor> triggerTypeDescriptorFactory);
}