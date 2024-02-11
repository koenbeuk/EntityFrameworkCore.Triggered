using EntityFrameworkCore.Triggered.Infrastructure.Internal;

namespace EntityFrameworkCore.Triggered.Internal;

public sealed class TriggerTypeRegistry<TTriggerTypeDescriptor>(Type entityType, Func<Type, TTriggerTypeDescriptor> triggerTypeDescriptorFactory)
{
    readonly Type _entityType = entityType;
    readonly Func<Type, TTriggerTypeDescriptor> _triggerTypeDescriptorFactory = triggerTypeDescriptorFactory;

    TTriggerTypeDescriptor[]? _resolvedDescriptors;

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
