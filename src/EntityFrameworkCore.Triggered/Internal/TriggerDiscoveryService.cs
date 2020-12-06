using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerDiscoveryService : ITriggerDiscoveryService, IResettableService
    {
        readonly static TriggerDescriptorComparer _triggerDescriptorComparer = new();

        readonly ITriggerServiceProviderAccessor _triggerServiceProviderAccessor;
        readonly ITriggerTypeRegistryService _triggerTypeRegistryService;

        IServiceProvider? _serviceProvider;

        public TriggerDiscoveryService(ITriggerServiceProviderAccessor triggerServiceProviderAccessor, ITriggerTypeRegistryService triggerTypeRegistryService)
        {
            _triggerServiceProviderAccessor = triggerServiceProviderAccessor ?? throw new ArgumentNullException(nameof(triggerServiceProviderAccessor));
            _triggerTypeRegistryService = triggerTypeRegistryService ?? throw new ArgumentNullException(nameof(triggerTypeRegistryService));
        }

        public IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
        {
            IServiceProvider serviceProvider = ServiceProvider;

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
                    var triggers = serviceProvider.GetServices(triggerTypeDescriptor.TriggerType);
                    foreach (var trigger in triggers)
                    {
                        if (triggerDescriptors == null)
                        {
                            triggerDescriptors = new List<TriggerDescriptor>(triggers.Count());
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
