using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerService : ITriggerService, IResettableService
    {
        readonly ITriggerDiscoveryService _triggerDiscoveryService;
        readonly IRecursionStrategy _recursionStrategy;
        readonly ILoggerFactory _loggerFactory;
        readonly TriggerOptions _options;

        ITriggerSession? _currentTriggerSession;

        public TriggerService(ITriggerDiscoveryService triggerDiscoveryService, IRecursionStrategy recursionStrategy, ILoggerFactory loggerFactory, IOptionsSnapshot<TriggerOptions> triggerOptionsSnapshot)
        {
            _triggerDiscoveryService = triggerDiscoveryService ?? throw new ArgumentNullException(nameof(triggerDiscoveryService));
            _recursionStrategy = recursionStrategy ?? throw new ArgumentNullException(nameof(recursionStrategy));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _options = triggerOptionsSnapshot.Value;
        }

        public ITriggerSession? Current
        {
            get => _currentTriggerSession;
            set => _currentTriggerSession = value;
        }

        public ITriggerSession CreateSession(DbContext context, IServiceProvider? serviceProvider)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var triggerContextTracker = new TriggerContextTracker(context.ChangeTracker, _recursionStrategy);

            if (serviceProvider != null)
            {
                _triggerDiscoveryService.SetServiceProvider(serviceProvider);
            }

            var triggerSession = new TriggerSession(this, _options, _triggerDiscoveryService, triggerContextTracker, _loggerFactory.CreateLogger<TriggerSession>());

            _currentTriggerSession = triggerSession;

            return triggerSession;
        }

        public void ResetState() => _currentTriggerSession?.Dispose();
        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            _currentTriggerSession?.Dispose();

            return Task.CompletedTask;
        }
    }
}
