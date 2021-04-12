using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{

    public interface ITriggerInstanceFactory
    {
        object Create(IServiceProvider serviceProvider);
    }

    public interface ITriggerInstanceFactory<out TTriggerType> : ITriggerInstanceFactory
    {

    }

    public sealed class TriggerInstanceFactory<TTriggerType> : ITriggerInstanceFactory<TTriggerType>
    {
        readonly TTriggerType? _serviceInstance;

        public TriggerInstanceFactory(TTriggerType? serviceInstance)
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
