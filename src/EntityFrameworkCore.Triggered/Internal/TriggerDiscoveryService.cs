using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerDiscoveryService : ITriggerDiscoveryService, IResettableService
    {
        readonly static TriggerDescriptorComparer _triggerDescriptorComparer = new();
        readonly ITriggerServiceProviderAccessor _triggerServiceProviderAccessor;
        readonly ITriggerTypeRegistryService _triggerTypeRegistryService;
        readonly TriggerFactory _triggerFactory;

        IServiceProvider? _serviceProvider;

        public TriggerDiscoveryService(ITriggerServiceProviderAccessor triggerServiceProviderAccessor, ITriggerTypeRegistryService triggerTypeRegistryService, TriggerFactory triggerFactory)
        {
            _triggerServiceProviderAccessor = triggerServiceProviderAccessor;
            _triggerTypeRegistryService = triggerTypeRegistryService ?? throw new ArgumentNullException(nameof(triggerTypeRegistryService));
            _triggerFactory = triggerFactory;
        }

        public IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
        {
            var registry = _triggerTypeRegistryService.ResolveRegistry(openTriggerType, entityType, triggerTypeDescriptorFactory);

            var triggerTypeDescriptors = registry.GetTriggerTypeDescriptors();
            if (triggerTypeDescriptors.Length == 0)
            {
                return Enumerable.Empty<TriggerDescriptor>();
            }
            else
            {
                List<TriggerDescriptor>? triggerDescriptors = null;

                foreach (var triggerTypeDescriptor in triggerTypeDescriptors)
                {
                    var triggers = _triggerFactory.Resolve(ServiceProvider, triggerTypeDescriptor.TriggerType);
                    foreach (var trigger in triggers)
                    {
                        if (triggerDescriptors == null)
                        {
                            triggerDescriptors = new List<TriggerDescriptor>();
                        }

                        if (trigger != null)
                        {
                            triggerDescriptors.Add(new TriggerDescriptor(triggerTypeDescriptor, trigger));
                        }
                    }
                }

                if (triggerDescriptors == null)
                {
                    return Enumerable.Empty<TriggerDescriptor>();
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
                if (_serviceProvider == null)
                {
                    _serviceProvider = _triggerServiceProviderAccessor.GetTriggerServiceProvider();
                }

                return _serviceProvider;
            }
            set => _serviceProvider = value;
        }

        public void ResetState()
        {
            _serviceProvider = null;
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            ResetState();
            return Task.CompletedTask;
        }
    }
}
