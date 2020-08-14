using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerTypeRegistryService : ITriggerTypeRegistryService
    {
        readonly ConcurrentDictionary<(Type, Type), TriggerTypeRegistry> _resolvedRegistries = new ConcurrentDictionary<(Type, Type), TriggerTypeRegistry>();

        TriggerTypeRegistry CreateRegistry(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
        {
            return _resolvedRegistries.GetOrAdd((openTriggerType, entityType), _ => new TriggerTypeRegistry(entityType, triggerTypeDescriptorFactory));
        }

        public TriggerTypeRegistry ResolveRegistry(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
        {
            if (_resolvedRegistries.TryGetValue((openTriggerType, entityType), out var registry))
            {
                return registry;
            }
            else
            {
                // Use an extra method to prevent allocation of a DispayClass in this method
                return CreateRegistry(openTriggerType, entityType, triggerTypeDescriptorFactory);
            }
        }
    }
}
