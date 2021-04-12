using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerFactory
    {
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
                Debug.Assert(trigger is not null);

                yield return trigger;
            }

            // Alternatively, triggers may be registered with the extension configuration
            var triggerServiceFactories = _internalServiceProvider.GetServices(typeof(ITriggerInstanceFactory<>).MakeGenericType(triggerType)).Cast<ITriggerInstanceFactory>();
            foreach (var triggerServiceFactory in triggerServiceFactories)
            {
                yield return triggerServiceFactory.Create(serviceProvider);
            }
        }
    }
}
