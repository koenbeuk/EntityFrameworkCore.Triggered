using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerDiscoveryService : ITriggerDiscoveryService
    {
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
            IServiceProvider serviceProvider;

            if (_serviceProvider == null)
            {
                serviceProvider = _triggerServiceProviderAccessor.GetTriggerServiceProvider();
            }
            else
            {
                serviceProvider = _serviceProvider;
            }

            var registry = _triggerTypeRegistryService.ResolveRegistry(openTriggerType, entityType, triggerTypeDescriptorFactory);

            var triggerTypeDescriptors = registry.GetTriggerTypeDescriptors();
            if (triggerTypeDescriptors.Length == 0)
            {
                return Enumerable.Empty<TriggerDescriptor>();
            }
            else
            {
                var triggerDescriptors = ImmutableSortedSet<TriggerDescriptor>.Empty;

                foreach (var triggerTypeDescriptor in triggerTypeDescriptors)
                {
                    var triggers = serviceProvider.GetServices(triggerTypeDescriptor.TriggerType);
                    foreach (var trigger in triggers)
                    {
                        triggerDescriptors = triggerDescriptors.Add(new TriggerDescriptor(triggerTypeDescriptor, trigger));
                    }
                }

                return triggerDescriptors;
            }
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            if (_serviceProvider != null)
            {
                throw new InvalidOperationException("Service provider needs to be set before discovery has started");
            }

            _serviceProvider = serviceProvider;
        }
    }
}
