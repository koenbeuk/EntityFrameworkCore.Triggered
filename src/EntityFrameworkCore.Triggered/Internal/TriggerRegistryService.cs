using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerRegistryService : ITriggerRegistryService, IResettableService
    {
        readonly ITriggerServiceProviderAccessor _triggerServiceProviderAccessor;

        private Dictionary<Type, TriggerRegistry>? _resolvedRegistries;

        public TriggerRegistryService(ITriggerServiceProviderAccessor triggerServiceProviderAccessor)
        {
            _triggerServiceProviderAccessor = triggerServiceProviderAccessor ?? throw new ArgumentNullException(nameof(triggerServiceProviderAccessor));
        }

        public TriggerRegistry GetRegistry(Type triggerType, Func<object, TriggerAdapterBase> executionStrategyFactory)
        {
            if (_resolvedRegistries == null)
            {
                _resolvedRegistries = new Dictionary<Type, TriggerRegistry>();
            }

            if (!_resolvedRegistries.TryGetValue(triggerType, out var registry))
            {
                registry = new TriggerRegistry(triggerType, _triggerServiceProviderAccessor.GetTriggerServiceProvider(), executionStrategyFactory);
                _resolvedRegistries[triggerType] = registry;
            }

            return registry;
        }

        public void ResetState()
        {
            _resolvedRegistries = null;
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            _resolvedRegistries = null;
            return Task.CompletedTask;
        }
    }
}
