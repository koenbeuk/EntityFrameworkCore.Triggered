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
        readonly ITriggerDiscoveryService _triggerDiscoveryService;
        readonly TriggerContextTracker _tracker;
        readonly ILogger<TriggerSession> _logger;

        public TriggerSession(TriggerOptions options, ITriggerDiscoveryService triggerDiscoveryService, TriggerContextTracker tracker, ILogger<TriggerSession> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _triggerDiscoveryService = triggerDiscoveryService ?? throw new ArgumentNullException(nameof(ITriggerDiscoveryService));
            _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void DiscoverChanges() 
            => _tracker.DiscoverChanges().Count();

        public async Task RaiseTriggers(Type openTriggerType, ITriggerContextDiscoveryStrategy triggerContextDiscoveryStrategy, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory, CancellationToken cancellationToken)
        {
            if (triggerContextDiscoveryStrategy == null)
            {
                throw new ArgumentNullException(nameof(triggerContextDiscoveryStrategy));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var triggerContextDescriptors = triggerContextDiscoveryStrategy.Discover(_options, _tracker, _logger);

            foreach (var triggerContextDescriptor in triggerContextDescriptors)
            {
                var triggerDescriptors = _triggerDiscoveryService
                    .DiscoverTriggers(openTriggerType, triggerContextDescriptor.EntityType, triggerTypeDescriptorFactory)
                    .ToList();

                _logger.LogDebug("Discovered {triggers} triggers for change of type {entityType}", triggerDescriptors.Count(), triggerContextDescriptor.EntityType);

                foreach (var triggerDescriptor in triggerDescriptors)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    _logger.LogInformation("Invoking trigger: {trigger} as {triggerType}", triggerDescriptor.Trigger.GetType().Name, triggerDescriptor.TypeDescriptor.TriggerType.Name);
                    await triggerDescriptor.Invoke(triggerContextDescriptor.GetTriggerContext(), cancellationToken).ConfigureAwait(false);
                }
            }
        }


        public Task RaiseBeforeSaveTriggers(CancellationToken cancellationToken)
        {
            if (_beforeSaveTriggerContextDiscoveryStrategy == null)
            {
                _beforeSaveTriggerContextDiscoveryStrategy = new RecursiveTriggerContextDiscoveryStrategy("BeforeSave");
            }

            return RaiseTriggers(typeof(IBeforeSaveTrigger<>), _beforeSaveTriggerContextDiscoveryStrategy, entityType => new BeforeSaveTriggerDescriptor(entityType), cancellationToken);  
        }

        public Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default)
        {
            if (_afterSaveTriggerContextDiscoveryStrategy == null)
            {
                _afterSaveTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("AfterSave");
            }

            return RaiseTriggers(typeof(IAfterSaveTrigger<>), _afterSaveTriggerContextDiscoveryStrategy, entityType => new AfterSaveTriggerDescriptor(entityType), cancellationToken);
        }
    }
}
