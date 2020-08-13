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
        static ITriggerContextDiscoveryStrategy? _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges; // To satisfy RaiseBeforeSaveTrigger's overload
        static ITriggerContextDiscoveryStrategy? _afterSaveTriggerContextDiscoveryStrategy;

        readonly TriggerOptions _options;
        readonly ITriggerDiscoveryService _triggerDiscoveryService;
        readonly TriggerContextTracker _tracker;
        readonly ILogger<TriggerSession> _logger;

        bool _raiseBeforeSaveTriggersCalled;

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

            var triggerContextDescriptorBatches = triggerContextDiscoveryStrategy.Discover(_options, _tracker, _logger);
            foreach (var triggerContextDescriptorBatch in triggerContextDescriptorBatches)
            {
                IEnumerable<(TriggerContextDescriptor triggerContextDescriptor, TriggerDescriptor triggerDescriptor)> triggerInvocations = triggerContextDescriptorBatch
                    .SelectMany(triggerContextDescriptor =>
                        _triggerDiscoveryService
                            .DiscoverTriggers(openTriggerType, triggerContextDescriptor.EntityType, triggerTypeDescriptorFactory)
                            .Select(triggerDescriptor => (triggerContextDescriptor, triggerDescriptor))
                    )
                    .OrderBy(x => x.triggerDescriptor.Priority);

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    triggerInvocations = triggerInvocations.ToList();
                    _logger.LogDebug("Discovered {triggers} triggers of type {openTriggerType}", triggerInvocations.Count(), openTriggerType);
                }

                foreach (var triggerInvocation in triggerInvocations)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Invoking trigger: {trigger} as {triggerType}", triggerInvocation.triggerDescriptor.GetType().Name, triggerInvocation.triggerDescriptor.TypeDescriptor.TriggerType.Name);
                    }

                    await triggerInvocation.triggerDescriptor.Invoke(triggerInvocation.triggerContextDescriptor.GetTriggerContext(), cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public Task RaiseBeforeSaveTriggers(CancellationToken cancellationToken)
            => RaiseBeforeSaveTriggers(_raiseBeforeSaveTriggersCalled, cancellationToken);

        public Task RaiseBeforeSaveTriggers(bool skipDetectedChanges, CancellationToken cancellationToken)
        {
            _raiseBeforeSaveTriggersCalled = true;

            ITriggerContextDiscoveryStrategy? strategy;

            if (skipDetectedChanges)
            {
                if (_beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges == null)
                {
                    _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges = new RecursiveTriggerContextDiscoveryStrategy("BeforeSave", true);
                }

                strategy = _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges;
            }
            else
            {
                if (_beforeSaveTriggerContextDiscoveryStrategy == null)
                {
                    _beforeSaveTriggerContextDiscoveryStrategy = new RecursiveTriggerContextDiscoveryStrategy("BeforeSave", false);
                }

                strategy = _beforeSaveTriggerContextDiscoveryStrategy;
            }

            _raiseBeforeSaveTriggersCalled = true;
            return RaiseTriggers(typeof(IBeforeSaveTrigger<>), strategy, entityType => new BeforeSaveTriggerDescriptor(entityType), cancellationToken);  
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
