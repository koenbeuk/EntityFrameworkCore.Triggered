using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerTypeRegistry
    {
        readonly Type _entityType;
        readonly Func<Type, ITriggerTypeDescriptor> _triggerTypeDescriptorFactory;

        ITriggerTypeDescriptor[]? _resolvedDescriptors;

        public TriggerTypeRegistry(Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
        {
            _entityType = entityType;
            _triggerTypeDescriptorFactory = triggerTypeDescriptorFactory;
        }

        IEnumerable<Type> GetEntityTypeHierarchy()
        {
            // Enumerable of the type hierarchy from base to concrete
            var typeHierarchy = TypeHelpers.EnumerateTypeHierarchy(_entityType).Reverse();
            foreach (var type in typeHierarchy)
            {
                foreach (var interfaceType in type.GetInterfaces())
                {
                    yield return interfaceType;

                }

                yield return type;
            }
        }

        public ITriggerTypeDescriptor[] GetTriggerTypeDescriptors()
        {
            if (_resolvedDescriptors == null)
            {
                var result = new List<ITriggerTypeDescriptor>();

                foreach (var triggerType in GetEntityTypeHierarchy().Distinct())
                {
                    var descriptor = _triggerTypeDescriptorFactory(triggerType);
                    result.Add(descriptor);
                }

                _resolvedDescriptors = result.ToArray();
            }

            return _resolvedDescriptors;
        }
    }
}
