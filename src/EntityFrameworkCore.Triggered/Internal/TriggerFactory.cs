using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerFactory
    {
        static readonly ConcurrentDictionary<Type, Type> _instanceFactoryTypeCache = new();
        readonly IServiceProvider _internalServiceProvider;

        public TriggerFactory(IServiceProvider internalServiceProvider)
        {
            _internalServiceProvider = internalServiceProvider;
        }

        public IEnumerable<object> Resolve(IServiceProvider serviceProvider, Type triggerType)
        {
            // triggers may be directly registered with our DI container
            var triggers = serviceProvider.GetServices(triggerType);
            foreach (var trigger in triggers)
            {
                if (trigger is not null)
                {
                    yield return trigger;
                }
            }

            // Alternatively, triggers may be registered with the extension configuration
            var instanceFactoryType = _instanceFactoryTypeCache.GetOrAdd(triggerType,
                triggerType => typeof(ITriggerInstanceFactory<>).MakeGenericType(triggerType)
            );

            var triggerServiceFactories = _internalServiceProvider.GetServices(instanceFactoryType);
            if (triggerServiceFactories.Any())
            {
                foreach (var triggerServiceFactory in triggerServiceFactories)
                {
                    if (triggerServiceFactory is not null)
                    {
                        yield return ((ITriggerInstanceFactory)triggerServiceFactory).Create(serviceProvider);
                    }
                }
            }
        }
    }
}
