using System;
using System.Collections.Concurrent;
using EntityFrameworkCore.Triggered.Internal.Descriptors;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerTypeRegistryService : ITriggerTypeRegistryService
    {
        readonly ConcurrentDictionary<(Type, Type, Type), object> _resolvedRegistries = new();

        TriggerTypeRegistry<TTriggerTypeDescriptor> CreateRegistry<TTriggerTypeDescriptor>(Type openTriggerType, Type entityType, Func<Type, TTriggerTypeDescriptor> triggerTypeDescriptorFactory)
            => (TriggerTypeRegistry<TTriggerTypeDescriptor>)_resolvedRegistries.GetOrAdd((openTriggerType, entityType, typeof(TTriggerTypeDescriptor)), _ => new TriggerTypeRegistry<TTriggerTypeDescriptor>(entityType, triggerTypeDescriptorFactory));

        public TriggerTypeRegistry<TTriggerTypeDescriptor> ResolveRegistry<TTriggerTypeDescriptor>(Type openTriggerType, Type entityType, Func<Type, TTriggerTypeDescriptor> triggerTypeDescriptorFactory)
        {
            if (_resolvedRegistries.TryGetValue((openTriggerType, entityType, typeof(TTriggerTypeDescriptor)), out var registry))
            {
                return (TriggerTypeRegistry<TTriggerTypeDescriptor>)registry;
            }
            else
            {
                // Use an extra method to prevent allocation of a DispayClass in this method
                return CreateRegistry(openTriggerType, entityType, triggerTypeDescriptorFactory);
            }
        }
    }
}
