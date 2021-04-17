using System;
using System.Collections.Concurrent;
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
        static ObjectFactory? _internalFactory;

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

            if (_internalFactory is null)
            {
                _internalFactory = ActivatorUtilities.CreateFactory(typeof(TTriggerType), Array.Empty<Type>());
            }

            _instance = _internalFactory(serviceProvider, null);
            return _instance;
        }
    }
}
