using EntityFrameworkCore.Triggered.Internal.Descriptors;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered.Internal;

public sealed class TriggerDiscoveryService(ITriggerServiceProviderAccessor triggerServiceProviderAccessor, ITriggerTypeRegistryService triggerTypeRegistryService, TriggerFactory triggerFactory) : ITriggerDiscoveryService, IResettableService
{
    readonly static TriggerDescriptorComparer _triggerDescriptorComparer = new();
    readonly ITriggerServiceProviderAccessor _triggerServiceProviderAccessor = triggerServiceProviderAccessor;
    readonly ITriggerTypeRegistryService _triggerTypeRegistryService = triggerTypeRegistryService ?? throw new ArgumentNullException(nameof(triggerTypeRegistryService));
    readonly TriggerFactory _triggerFactory = triggerFactory;

    IServiceProvider? _serviceProvider;

    public IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
    {
        var registry = _triggerTypeRegistryService.ResolveRegistry(openTriggerType, entityType, triggerTypeDescriptorFactory);

        var triggerTypeDescriptors = registry.GetTriggerTypeDescriptors();
        if (triggerTypeDescriptors.Length == 0)
        {
            return [];
        }
        else
        {
            List<TriggerDescriptor>? triggerDescriptors = null;

            foreach (var triggerTypeDescriptor in triggerTypeDescriptors)
            {
                var triggers = _triggerFactory.Resolve(ServiceProvider, triggerTypeDescriptor.TriggerType);
                foreach (var trigger in triggers)
                {
                    triggerDescriptors ??= [];

                    if (trigger != null)
                    {
                        triggerDescriptors.Add(new TriggerDescriptor(triggerTypeDescriptor, trigger));
                    }
                }
            }

            if (triggerDescriptors == null)
            {
                return [];
            }
            else
            {
                triggerDescriptors.Sort(_triggerDescriptorComparer);
                return triggerDescriptors;
            }
        }
    }

    public IEnumerable<AsyncTriggerDescriptor> DiscoverAsyncTriggers(Type openTriggerType, Type entityType, Func<Type, IAsyncTriggerTypeDescriptor> triggerTypeDescriptorFactory)
    {
        var registry = _triggerTypeRegistryService.ResolveRegistry(openTriggerType, entityType, triggerTypeDescriptorFactory);

        var triggerTypeDescriptors = registry.GetTriggerTypeDescriptors();
        if (triggerTypeDescriptors.Length == 0)
        {
            return [];
        }
        else
        {
            List<AsyncTriggerDescriptor>? triggerDescriptors = null;

            foreach (var triggerTypeDescriptor in triggerTypeDescriptors)
            {
                var triggers = _triggerFactory.Resolve(ServiceProvider, triggerTypeDescriptor.TriggerType);
                foreach (var trigger in triggers)
                {
                    triggerDescriptors ??= [];

                    if (trigger != null)
                    {
                        triggerDescriptors.Add(new AsyncTriggerDescriptor(triggerTypeDescriptor, trigger));
                    }
                }
            }

            if (triggerDescriptors == null)
            {
                return [];
            }
            else
            {
                triggerDescriptors.Sort(_triggerDescriptorComparer);
                return triggerDescriptors;
            }
        }
    }

    public IEnumerable<TTrigger> DiscoverTriggers<TTrigger>()
    {
        // We can skip the registry as there is no generic argument
        var triggers = _triggerFactory.Resolve(ServiceProvider, typeof(TTrigger));

        return triggers
            .Select((trigger, index) => (
                trigger,
                defaultPriority: index,
                customPriority: (trigger as ITriggerPriority)?.Priority ?? 0
            ))
            .OrderBy(x => x.customPriority)
            .ThenBy(x => x.defaultPriority)
            .Select(x => x.trigger)
            .Cast<TTrigger>();
    }

    public IServiceProvider ServiceProvider
    {
        get
        {
            _serviceProvider ??= _triggerServiceProviderAccessor.GetTriggerServiceProvider();

            return _serviceProvider;
        }
        set => _serviceProvider = value;
    }

    public void ResetState() => _serviceProvider = null;

    public Task ResetStateAsync(CancellationToken cancellationToken = default)
    {
        ResetState();
        return Task.CompletedTask;
    }
}
