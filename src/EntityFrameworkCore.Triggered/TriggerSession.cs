using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerSession : ITriggerSession
    {
        static ITriggerContextDiscoveryStrategy? _beforeSaveTriggerContextDiscoveryStrategy;
        static ITriggerContextDiscoveryStrategy? _afterSaveTriggerContextDiscoveryStrategy;

        readonly TriggerOptions _options;
        readonly ITriggerRegistryService _triggerRegistryService;
        readonly TriggerContextTracker _tracker;
        readonly ILogger<TriggerSession> _logger;

        public TriggerSession(TriggerOptions options, ITriggerRegistryService triggerRegistryService, TriggerContextTracker tracker, ILogger<TriggerSession> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _triggerRegistryService = triggerRegistryService ?? throw new ArgumentNullException(nameof(TriggerRegistry));
            _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void DiscoverChanges() 
            => _tracker.DiscoverChanges().Count();

        public async Task RaiseTriggers(ITriggerContextDiscoveryStrategy discoveryStrategy, CancellationToken cancellationToken)
        {
            if (discoveryStrategy == null)
            {
                throw new ArgumentNullException(nameof(discoveryStrategy));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var discoveryResult = discoveryStrategy.Discover(_options, _triggerRegistryService, _tracker, _logger);

            foreach (var (triggerAdapter, triggerContextDescriptor) in discoveryResult)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _logger.LogInformation("Invoking trigger: {trigger}", triggerAdapter.Trigger.GetType().FullName);
                await triggerAdapter.Execute(triggerContextDescriptor.GetTriggerContext(), cancellationToken).ConfigureAwait(false);
            }
        }


        public Task RaiseBeforeSaveTriggers(CancellationToken cancellationToken)
        {
            if (_beforeSaveTriggerContextDiscoveryStrategy == null)
            {
                _beforeSaveTriggerContextDiscoveryStrategy = new RecursiveTriggerContextDiscoveryStrategy("BeforeSave", typeof(IBeforeSaveTrigger<>), trigger => new BeforeSaveTriggerAdapter(trigger));
            }

            return RaiseTriggers(_beforeSaveTriggerContextDiscoveryStrategy, cancellationToken);  
        }

        public Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default)
        {
            if (_afterSaveTriggerContextDiscoveryStrategy == null)
            {
                _afterSaveTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("AfterSave", typeof(IAfterSaveTrigger<>), trigger => new AfterSaveTriggerAdapter(trigger));
            }

            return RaiseTriggers(_afterSaveTriggerContextDiscoveryStrategy, cancellationToken);
        }
    }
}
