using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.CascadeStrategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerService : ITriggerService, IResettableService
    {

        readonly ITriggerDiscoveryService _triggerDiscoveryService;
        readonly ICascadeStrategy _cascadingStrategy;
        readonly ILoggerFactory _loggerFactory;
        readonly TriggerSessionConfiguration _defaultConfiguration;

        ITriggerSession? _currentTriggerSession;

        public TriggerService(ITriggerDiscoveryService triggerDiscoveryService, ICascadeStrategy cascadingStrategy, ILoggerFactory loggerFactory, IOptions<TriggerOptions> triggerOptions)
        {
            _triggerDiscoveryService = triggerDiscoveryService ?? throw new ArgumentNullException(nameof(triggerDiscoveryService));
            _cascadingStrategy = cascadingStrategy ?? throw new ArgumentNullException(nameof(cascadingStrategy));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _defaultConfiguration = new TriggerSessionConfiguration(false, triggerOptions.Value.MaxCascadeCycles);
            
            Configuration = _defaultConfiguration;
        }

        public ITriggerSession? Current
        {
            get => _currentTriggerSession;
            set => _currentTriggerSession = value;
        }

        public TriggerSessionConfiguration Configuration { get; set; }

        public ITriggerSession CreateSession(DbContext context, IServiceProvider? serviceProvider)
            => CreateSession(context, Configuration, serviceProvider);

        public ITriggerSession CreateSession(DbContext context, TriggerSessionConfiguration configuration, IServiceProvider? serviceProvider)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var triggerContextTracker = new TriggerContextTracker(context.ChangeTracker, _cascadingStrategy);

            if (serviceProvider != null)
            {
                _triggerDiscoveryService.ServiceProvider = serviceProvider;
            }

            var triggerSession = new TriggerSession(this, configuration, _triggerDiscoveryService, triggerContextTracker, _loggerFactory.CreateLogger<TriggerSession>());

            _currentTriggerSession = triggerSession;

            return triggerSession;
        }

        public void ResetState()
        {
            if (_currentTriggerSession != null)
            {
                _currentTriggerSession.Dispose();
                _currentTriggerSession = null;
            }

            Configuration = _defaultConfiguration;
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            ResetState();

            return Task.CompletedTask;
        }
    }
}
