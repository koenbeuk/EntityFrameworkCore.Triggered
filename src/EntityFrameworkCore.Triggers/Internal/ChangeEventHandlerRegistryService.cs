using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggers.Internal
{
    public sealed class ChangeEventHandlerRegistryService : IChangeEventHandlerRegistryService
    {
        readonly ConcurrentDictionary<Type, ChangeEventHandlerRegistry> _cachedRegistries = new ConcurrentDictionary<Type, ChangeEventHandlerRegistry>();

        readonly IServiceProvider _serviceProvider;

        public ChangeEventHandlerRegistryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ChangeEventHandlerRegistry GetRegistry(Type changeHandlerType, Func<object, ChangeEventHandlerExecutionAdapterBase> executionStrategyFactory) 
            => _cachedRegistries.GetOrAdd(changeHandlerType, _ => new ChangeEventHandlerRegistry(changeHandlerType, _serviceProvider, executionStrategyFactory));
    }
}
