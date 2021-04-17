using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        object? _instance;

        public TriggerInstanceFactory(object? instance)
        {
            _instance = instance;
        }

        public object Create(IServiceProvider serviceProvider)
        {
            if (_instance is not null)
            {
                return _instance;
            }

            // todo: create object factory and cache
            _instance = ActivatorUtilities.CreateInstance(serviceProvider, typeof(TTriggerType));

            return _instance;
        }
    }
}
