using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerRegistryService : ITriggerRegistryService
    {
        readonly IServiceProvider _serviceProvider;
        readonly IServiceProvider? _applicationServiceProvider;

        private Dictionary<Type, TriggerRegistry>? _resolvedRegistries;

        public TriggerRegistryService(IServiceProvider serviceProvider, IServiceProvider? applicationServiceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _applicationServiceProvider = applicationServiceProvider;
        }

        public TriggerRegistry GetRegistry(Type triggerType, Func<object, TriggerAdapterBase> executionStrategyFactory)
        {
            if (_resolvedRegistries == null)
            {
                _resolvedRegistries = new Dictionary<Type, TriggerRegistry>();
            }

            if (!_resolvedRegistries.TryGetValue(triggerType, out var registry))
            {
                registry = new TriggerRegistry(triggerType, _serviceProvider, _applicationServiceProvider, executionStrategyFactory);
                _resolvedRegistries[triggerType] = registry;
            }

            return registry;
        }
    }
}
