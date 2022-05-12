using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Lifecycles;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerSession : ITriggerSession
    {
        static ITriggerContextDiscoveryStrategy? _beforeSaveTriggerContextDiscoveryStrategy;
        static ITriggerContextDiscoveryStrategy? _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges; // To satisfy RaiseBeforeSaveTrigger's overload
        static ITriggerContextDiscoveryStrategy? _afterSaveTriggerContextDiscoveryStrategy;
        static ITriggerContextDiscoveryStrategy? _afterSaveFailedTriggerContextDiscoveryStrategy;

        readonly ITriggerService _triggerService;
        readonly TriggerSessionConfiguration _configuration;
        readonly ITriggerDiscoveryService _triggerDiscoveryService;
        readonly TriggerContextTracker _tracker;
        readonly ILogger<TriggerSession> _logger;

        readonly EntityBagStateManager _entityBagStateManager = new();

        bool _raiseBeforeSaveTriggersCalled;

        public TriggerSession(ITriggerService triggerService, TriggerSessionConfiguration configuration, ITriggerDiscoveryService triggerDiscoveryService, TriggerContextTracker tracker, ILogger<TriggerSession> logger)
        {
            _triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _triggerDiscoveryService = triggerDiscoveryService ?? throw new ArgumentNullException(nameof(triggerDiscoveryService));
            _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TriggerContextTracker Tracker => _tracker;

        public ITriggerDiscoveryService DiscoveryService => _triggerDiscoveryService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Incorrect warning")]
        public void DiscoverChanges()
            => _tracker.DiscoverChanges().Count();

        public async Task RaiseTriggers(Type openTriggerType, Exception? exception, ITriggerContextDiscoveryStrategy triggerContextDiscoveryStrategy, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory, CancellationToken cancellationToken)
        {
            if (triggerContextDiscoveryStrategy == null)
            {
                throw new ArgumentNullException(nameof(triggerContextDiscoveryStrategy));
            }

            if (_configuration.Disabled)
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var triggerContextDescriptorBatches = triggerContextDiscoveryStrategy.Discover(_configuration, _tracker, _logger);
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
                            _logger.LogInformation("Invoking trigger: {trigger} as {triggerType}", triggerDescriptor.Trigger.GetType(), triggerDescriptor.TypeDescriptor.TriggerType);
                        }

                        await triggerDescriptor.Invoke(triggerContextDescriptor.GetTriggerContext(_entityBagStateManager), exception, cancellationToken).ConfigureAwait(false);
                    }
                }

            }
        }

        public async Task RaiseBeforeSaveStartingTriggers(CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IBeforeSaveStartingTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.BeforeSaveStarting(cancellationToken);
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
                    _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges = new CascadingTriggerContextDiscoveryStrategy("BeforeSave", true);
                }

                strategy = _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges;
            }
            else
            {
                if (_beforeSaveTriggerContextDiscoveryStrategy == null)
                {
                    _beforeSaveTriggerContextDiscoveryStrategy = new CascadingTriggerContextDiscoveryStrategy("BeforeSave", false);
                }

                strategy = _beforeSaveTriggerContextDiscoveryStrategy;
            }

            _raiseBeforeSaveTriggersCalled = true;
            return RaiseTriggers(typeof(IBeforeSaveTrigger<>), null, strategy, entityType => new BeforeSaveTriggerDescriptor(entityType), cancellationToken);
        }

        public async Task RaiseBeforeSaveCompletedTriggers(CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IBeforeSaveCompletedTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.BeforeSaveCompleted(cancellationToken);
            }
        }

        public void CaptureDiscoveredChanges() => _tracker.CaptureChanges();

        public async Task RaiseAfterSaveStartingTriggers(CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveStartingTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterSaveStarting(cancellationToken);
            }
        }

        public Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default)
        {
            if (_afterSaveTriggerContextDiscoveryStrategy == null)
            {
                _afterSaveTriggerContextDiscoveryStrategy = new NonCascadingTriggerContextDiscoveryStrategy("AfterSave");
            }

            return RaiseTriggers(typeof(IAfterSaveTrigger<>), null, _afterSaveTriggerContextDiscoveryStrategy, entityType => new AfterSaveTriggerDescriptor(entityType), cancellationToken);
        }

        public async Task RaiseAfterSaveCompletedTriggers(CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveCompletedTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterSaveCompleted(cancellationToken);
            }
        }

        public async Task RaiseAfterSaveFailedStartingTriggers(Exception exception, CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveFailedStartingTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterSaveFailedStarting(exception, cancellationToken);
            }
        }

        public Task RaiseAfterSaveFailedTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            if (_afterSaveFailedTriggerContextDiscoveryStrategy == null)
            {
                _afterSaveFailedTriggerContextDiscoveryStrategy = new NonCascadingTriggerContextDiscoveryStrategy("AfterSaveFailed");
            }

            return RaiseTriggers(typeof(IAfterSaveFailedTrigger<>), exception, _afterSaveFailedTriggerContextDiscoveryStrategy, entityType => new AfterSaveFailedTriggerDescriptor(entityType), cancellationToken);
        }

        public async Task RaiseAfterSaveFailedCompletedTriggers(Exception exception, CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveFailedCompletedTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterSaveFailedCompleted(exception, cancellationToken);
            }
        }

        public void Dispose()
        {
            if (_triggerService.Current == this)
            {
                _triggerService.Current = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
