using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerDiscoveryService : ITriggerDiscoveryService
    {
        readonly ITriggerServiceProviderAccessor _triggerServiceProviderAccessor;
        readonly ITriggerTypeRegistryService _triggerTypeRegistryService;

        public TriggerDiscoveryService(ITriggerServiceProviderAccessor triggerServiceProviderAccessor, ITriggerTypeRegistryService triggerTypeRegistryService)
        {
            _triggerServiceProviderAccessor = triggerServiceProviderAccessor ?? throw new ArgumentNullException(nameof(triggerServiceProviderAccessor));
            _triggerTypeRegistryService = triggerTypeRegistryService ?? throw new ArgumentNullException(nameof(triggerTypeRegistryService));
        }


        public IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
        {
            var registry = _triggerTypeRegistryService.ResolveRegistry(openTriggerType, entityType, triggerTypeDescriptorFactory);
            var serviceProvider = _triggerServiceProviderAccessor.GetTriggerServiceProvider();

            return registry.GetTriggerTypeDescriptors()
                .SelectMany(triggerTypeDescriptor =>
                    serviceProvider
                        .GetServices(triggerTypeDescriptor.TriggerType)
                        .Distinct()
                        .Select(trigger => new TriggerDescriptor(triggerTypeDescriptor, trigger))
                        .OrderBy(x => x.Priority)
                );
        }
    }
}
