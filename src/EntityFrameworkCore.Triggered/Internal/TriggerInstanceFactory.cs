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
        object Create(DbContext? dbContext, IServiceProvider serviceProvider);
    }

    public interface ITriggerInstanceFactory<out TTriggerType> : ITriggerInstanceFactory
    {

    }

    public sealed class TriggerInstanceFactory<TTriggerType> : ITriggerInstanceFactory<TTriggerType>
    {
        readonly object? _serviceInstance;

        public TriggerInstanceFactory(object? serviceInstance)
        {
            _serviceInstance = serviceInstance;
        }

        public object Create(DbContext? dbContext, IServiceProvider serviceProvider)
        {
            if (_serviceInstance is not null)
            {
                return _serviceInstance;
            }

            var arguments = dbContext is not null ? new object[] { dbContext } : Array.Empty<object>();

            return ActivatorUtilities.CreateInstance(serviceProvider, typeof(TTriggerType), arguments);
        }
    }
}
