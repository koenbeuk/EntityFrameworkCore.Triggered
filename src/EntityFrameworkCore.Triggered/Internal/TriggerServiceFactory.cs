using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{

    public interface ITriggerServiceFactory
    {
        object Create(IServiceProvider serviceProvider);
    }

    public interface ITriggerServiceFactory<out TTriggerType> : ITriggerServiceFactory
    {

    }

    public sealed class TriggerServiceFactory<TTriggerType> : ITriggerServiceFactory<TTriggerType>
    {
        readonly TTriggerType? _serviceInstance;

        public TriggerServiceFactory(TTriggerType? serviceInstance)
        {
            _serviceInstance = serviceInstance;
        }


        public object Create(IServiceProvider serviceProvider)
        {
            if (_serviceInstance is not null)
            {
                return _serviceInstance;
            }

            return ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, typeof(TTriggerType));
        }
    }
}
