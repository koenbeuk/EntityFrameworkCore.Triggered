using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerTypeRegistry<TTriggerTypeDescriptor>
    {
        readonly Type _entityType;
        readonly Func<Type, TTriggerTypeDescriptor> _triggerTypeDescriptorFactory;

        TTriggerTypeDescriptor[]? _resolvedDescriptors;

        public TriggerTypeRegistry(Type entityType, Func<Type, TTriggerTypeDescriptor> triggerTypeDescriptorFactory)
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

        public TTriggerTypeDescriptor[] GetTriggerTypeDescriptors()
        {
            if (_resolvedDescriptors == null)
            {
                var result = new List<TTriggerTypeDescriptor>();

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
