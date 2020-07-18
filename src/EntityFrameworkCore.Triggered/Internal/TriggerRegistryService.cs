using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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


        public TriggerRegistryService(IServiceProvider serviceProvider, IServiceProvider? applicationServiceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _applicationServiceProvider = applicationServiceProvider;
        }

        public TriggerRegistry GetRegistry(Type changeHandlerType, Func<object, TriggerAdapterBase> executionStrategyFactory) 
            => new TriggerRegistry(changeHandlerType, _serviceProvider, _applicationServiceProvider, executionStrategyFactory);
    }
}
