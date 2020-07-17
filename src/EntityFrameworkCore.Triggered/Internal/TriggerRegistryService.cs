using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerRegistryService : ITriggerRegistryService
    {
        readonly ConcurrentDictionary<Type, TriggerRegistry> _cachedRegistries = new ConcurrentDictionary<Type, TriggerRegistry>();

        readonly IServiceProvider _serviceProvider;

        public TriggerRegistryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TriggerRegistry GetRegistry(Type changeHandlerType, Func<object, TriggerAdapterBase> executionStrategyFactory) 
            => _cachedRegistries.GetOrAdd(changeHandlerType, _ => new TriggerRegistry(changeHandlerType, _serviceProvider, executionStrategyFactory));
    }
}
