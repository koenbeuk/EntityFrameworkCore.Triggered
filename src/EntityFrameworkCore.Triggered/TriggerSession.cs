﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerSession : ITriggerSession
    {
        static ITriggerContextDiscoveryStrategy? _beforeSaveTriggerContextDiscoveryStrategy;
        static ITriggerContextDiscoveryStrategy? _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges; // To satisfy RaiseBeforeSaveTrigger's overload
        static ITriggerContextDiscoveryStrategy? _afterSaveTriggerContextDiscoveryStrategy;
        static ITriggerContextDiscoveryStrategy? _afterSaveFailedTriggerContextDiscoveryStrategy;

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

        public async Task RaiseTriggers(Type openTriggerType, Exception? exception, ITriggerContextDiscoveryStrategy triggerContextDiscoveryStrategy, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory, CancellationToken cancellationToken)
        {
            if (triggerContextDiscoveryStrategy == null)
            {
                throw new ArgumentNullException(nameof(triggerContextDiscoveryStrategy));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var triggerContextDescriptorBatches = triggerContextDiscoveryStrategy.Discover(_options, _tracker, _logger);
            foreach (var triggerContextDescriptorBatch in triggerContextDescriptorBatches)
            {
                List<(TriggerContextDescriptor triggerContextDescriptor, TriggerDescriptor triggerDescriptor)>? triggerInvocations = null;

                foreach (var triggerContextDescriptor in triggerContextDescriptorBatch)
                {
                    var triggerDescriptors = _triggerDiscoveryService.DiscoverTriggers(openTriggerType, triggerContextDescriptor.EntityType, triggerTypeDescriptorFactory);

                    foreach (var triggerDescriptor in triggerDescriptors)
                    {
                        if (triggerInvocations == null)
                        {
                            triggerInvocations = new List<(TriggerContextDescriptor triggerContextDescriptor, TriggerDescriptor triggerDescriptor)>();
                        }

                        triggerInvocations.Add((triggerContextDescriptor, triggerDescriptor));
                    }
                }

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Discovered {triggers} triggers of type {openTriggerType}", triggerInvocations?.Count ?? 0, openTriggerType);
                }

                if (triggerInvocations != null)
                {
                    foreach (var (triggerContextDescriptor, triggerDescriptor) in triggerInvocations.OrderBy(x => x.triggerDescriptor.Priority))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Invoking trigger: {trigger} as {triggerType}", triggerDescriptor.Trigger.GetType().Name, triggerDescriptor.TypeDescriptor.TriggerType.Name);
                        }

                        await triggerDescriptor.Invoke(triggerContextDescriptor.GetTriggerContext(), exception, cancellationToken).ConfigureAwait(false);
                    }
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
            return RaiseTriggers(typeof(IBeforeSaveTrigger<>), null, strategy, entityType => new BeforeSaveTriggerDescriptor(entityType), cancellationToken);
        }

        public void CaptureDiscoveredChanges() => _tracker.CaptureChanges();

        public Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default)
        {
            if (_afterSaveTriggerContextDiscoveryStrategy == null)
            {
                _afterSaveTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("AfterSave");
            }

            return RaiseTriggers(typeof(IAfterSaveTrigger<>), null, _afterSaveTriggerContextDiscoveryStrategy, entityType => new AfterSaveTriggerDescriptor(entityType), cancellationToken);
        }

        public Task RaiseAfterSaveFailedTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            if (_afterSaveFailedTriggerContextDiscoveryStrategy == null)
            {
                _afterSaveFailedTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("AfterSaveFailed");
            }

            return RaiseTriggers(typeof(IAfterSaveFailedTrigger<>), exception, _afterSaveFailedTriggerContextDiscoveryStrategy, entityType => new AfterSaveFailedTriggerDescriptor(entityType, exception), cancellationToken);

        }
    }
}
