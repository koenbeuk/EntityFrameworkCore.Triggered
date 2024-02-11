using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;
using EntityFrameworkCore.Triggered.Lifecycles;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerSession(ITriggerService triggerService, TriggerSessionConfiguration configuration, ITriggerDiscoveryService triggerDiscoveryService, TriggerContextTracker tracker, ILogger<TriggerSession> logger) : ITriggerSession
    {
        static ITriggerContextDiscoveryStrategy? _beforeSaveTriggerContextDiscoveryStrategy;
        static ITriggerContextDiscoveryStrategy? _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges; // To satisfy RaiseBeforeSaveTrigger's overload
        static ITriggerContextDiscoveryStrategy? _afterSaveTriggerContextDiscoveryStrategy;
        static ITriggerContextDiscoveryStrategy? _afterSaveFailedTriggerContextDiscoveryStrategy;

        readonly ITriggerService _triggerService = triggerService ?? throw new ArgumentNullException(nameof(triggerService));
        readonly TriggerSessionConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        readonly ITriggerDiscoveryService _triggerDiscoveryService = triggerDiscoveryService ?? throw new ArgumentNullException(nameof(triggerDiscoveryService));
        readonly TriggerContextTracker _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
        readonly ILogger<TriggerSession> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        readonly EntityBagStateManager _entityBagStateManager = new();

        bool _raiseBeforeSaveTriggersCalled;
        bool _raiseBeforeSaveAsyncTriggersCalled;

        public TriggerContextTracker Tracker => _tracker;

        public ITriggerDiscoveryService DiscoveryService => _triggerDiscoveryService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Incorrect warning")]
        public void DiscoverChanges()
            => _tracker.DiscoverChanges().Count();

        public void RaiseTriggers(Type openTriggerType, Exception? exception, ITriggerContextDiscoveryStrategy triggerContextDiscoveryStrategy, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
        {
            ArgumentNullException.ThrowIfNull(triggerContextDiscoveryStrategy);

            if (_configuration.Disabled)
            {
                return;
            }

            var triggerContextDescriptorBatches = triggerContextDiscoveryStrategy.Discover(_configuration, _tracker, _logger);
            foreach (var triggerContextDescriptorBatch in triggerContextDescriptorBatches)
            {
                List<(TriggerContextDescriptor triggerContextDescriptor, TriggerDescriptor triggerDescriptor)>? triggerInvocations = null;

                foreach (var triggerContextDescriptor in triggerContextDescriptorBatch)
                {
                    var triggerDescriptors = _triggerDiscoveryService.DiscoverTriggers(openTriggerType, triggerContextDescriptor.EntityType, triggerTypeDescriptorFactory);

                    foreach (var triggerDescriptor in triggerDescriptors)
                    {
                        triggerInvocations ??= [];

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
                        if (_logger.IsEnabled(LogLevel.Information))
                        {
                            _logger.LogInformation("Invoking trigger: {trigger} as {triggerType}", triggerDescriptor.Trigger.GetType(), triggerDescriptor.TypeDescriptor.TriggerType);
                        }

                        triggerDescriptor.Invoke(triggerContextDescriptor.GetTriggerContext(_entityBagStateManager), exception);
                    }
                }

            }
        }

        public async Task RaiseAsyncTriggers(Type openTriggerType, Exception? exception, ITriggerContextDiscoveryStrategy triggerContextDiscoveryStrategy, Func<Type, IAsyncTriggerTypeDescriptor> triggerTypeDescriptorFactory, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(triggerContextDiscoveryStrategy);

            if (_configuration.Disabled)
            {
                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var triggerContextDescriptorBatches = triggerContextDiscoveryStrategy.Discover(_configuration, _tracker, _logger);
            foreach (var triggerContextDescriptorBatch in triggerContextDescriptorBatches)
            {
                List<(TriggerContextDescriptor triggerContextDescriptor, AsyncTriggerDescriptor triggerDescriptor)>? triggerInvocations = null;

                foreach (var triggerContextDescriptor in triggerContextDescriptorBatch)
                {
                    var triggerDescriptors = _triggerDiscoveryService.DiscoverAsyncTriggers(openTriggerType, triggerContextDescriptor.EntityType, triggerTypeDescriptorFactory);

                    foreach (var triggerDescriptor in triggerDescriptors)
                    {
                        triggerInvocations ??= [];

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

        public void RaiseBeforeSaveStartingTriggers()
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IBeforeSaveStartingTrigger>();

            foreach (var trigger in triggers)
            {
                trigger.BeforeSaveStarting();
            }
        }

        public async Task RaiseBeforeSaveStartingAsyncTriggers(CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IBeforeSaveStartingAsyncTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.BeforeSaveStartingAsync(cancellationToken);
            }
        }

        public void RaiseBeforeSaveTriggers()
            => RaiseBeforeSaveTriggers(_raiseBeforeSaveTriggersCalled);

        public void RaiseBeforeSaveTriggers(bool skipDetectedChanges)
        {
            _raiseBeforeSaveTriggersCalled = true;

            ITriggerContextDiscoveryStrategy? strategy;

            if (skipDetectedChanges)
            {
                _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges ??= new CascadingTriggerContextDiscoveryStrategy("BeforeSave", true);

                strategy = _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges;
            }
            else
            {
                _beforeSaveTriggerContextDiscoveryStrategy ??= new CascadingTriggerContextDiscoveryStrategy("BeforeSave", false);

                strategy = _beforeSaveTriggerContextDiscoveryStrategy;
            }

            _raiseBeforeSaveTriggersCalled = true;
            RaiseTriggers(typeof(IBeforeSaveTrigger<>), null, strategy, entityType => new BeforeSaveTriggerDescriptor(entityType));
        }

        public Task RaiseBeforeSaveAsyncTriggers(CancellationToken cancellationToken)
            => RaiseBeforeSaveAsyncTriggers(_raiseBeforeSaveAsyncTriggersCalled, cancellationToken);

        public Task RaiseBeforeSaveAsyncTriggers(bool skipDetectedChanges, CancellationToken cancellationToken)
        {
            _raiseBeforeSaveAsyncTriggersCalled = true;

            ITriggerContextDiscoveryStrategy? strategy;

            if (skipDetectedChanges)
            {
                _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges ??= new CascadingTriggerContextDiscoveryStrategy("BeforeSave", true);

                strategy = _beforeSaveTriggerContextDiscoveryStrategyWithSkipDetectedChanges;
            }
            else
            {
                _beforeSaveTriggerContextDiscoveryStrategy ??= new CascadingTriggerContextDiscoveryStrategy("BeforeSave", false);

                strategy = _beforeSaveTriggerContextDiscoveryStrategy;
            }

            _raiseBeforeSaveTriggersCalled = true;
            return RaiseAsyncTriggers(typeof(IBeforeSaveAsyncTrigger<>), null, strategy, entityType => new BeforeSaveAsyncTriggerDescriptor(entityType), cancellationToken);
        }

        public void RaiseBeforeSaveCompletedTriggers()
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IBeforeSaveCompletedTrigger>();

            foreach (var trigger in triggers)
            {
                trigger.BeforeSaveCompleted();
            }
        }

        public async Task RaiseBeforeSaveCompletedAsyncTriggers(CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IBeforeSaveCompletedAsyncTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.BeforeSaveCompletedAsync(cancellationToken);
            }
        }

        public void CaptureDiscoveredChanges() => _tracker.CaptureChanges();

        public void RaiseAfterSaveStartingTriggers()
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveStartingTrigger>();

            foreach (var trigger in triggers)
            {
                trigger.AfterSaveStarting();
            }
        }

        public async Task RaiseAfterSaveStartingAsyncTriggers(CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveStartingAsyncTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterSaveStartingAsync(cancellationToken);
            }
        }

        public void RaiseAfterSaveTriggers()
        {
            _afterSaveTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("AfterSave");

            RaiseTriggers(typeof(IAfterSaveTrigger<>), null, _afterSaveTriggerContextDiscoveryStrategy, entityType => new AfterSaveTriggerDescriptor(entityType));
        }

        public Task RaiseAfterSaveAsyncTriggers(CancellationToken cancellationToken = default)
        {
            _afterSaveTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("AfterSave");

            return RaiseAsyncTriggers(typeof(IAfterSaveAsyncTrigger<>), null, _afterSaveTriggerContextDiscoveryStrategy, entityType => new AfterSaveAsyncTriggerDescriptor(entityType), cancellationToken);
        }

        public void RaiseAfterSaveCompletedTriggers()
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveCompletedTrigger>();

            foreach (var trigger in triggers)
            {
                trigger.AfterSaveCompleted();
            }
        }

        public async Task RaiseAfterSaveCompletedAsyncTriggers(CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveCompletedAsyncTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterSaveCompletedAsync(cancellationToken);
            }
        }

        public void RaiseAfterSaveFailedStartingTriggers(Exception exception)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveFailedStartingTrigger>();

            foreach (var trigger in triggers)
            {
                trigger.AfterSaveFailedStarting(exception);
            }
        }

        public async Task RaiseAfterSaveFailedStartingAsyncTriggers(Exception exception, CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveFailedStartingAsyncTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterSaveFailedStartingAsync(exception, cancellationToken);
            }
        }

        public void RaiseAfterSaveFailedTriggers(Exception exception)
        {
            _afterSaveFailedTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("AfterSaveFailed");

            RaiseTriggers(typeof(IAfterSaveFailedTrigger<>), exception, _afterSaveFailedTriggerContextDiscoveryStrategy, entityType => new AfterSaveFailedTriggerDescriptor(entityType));
        }

        public Task RaiseAfterSaveFailedAsyncTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            _afterSaveFailedTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("AfterSaveFailed");

            return RaiseAsyncTriggers(typeof(IAfterSaveFailedAsyncTrigger<>), exception, _afterSaveFailedTriggerContextDiscoveryStrategy, entityType => new AfterSaveFailedAsyncTriggerDescriptor(entityType), cancellationToken);
        }

        public void RaiseAfterSaveFailedCompletedTriggers(Exception exception)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveFailedCompletedTrigger>();

            foreach (var trigger in triggers)
            {
                trigger.AfterSaveFailedCompleted(exception);
            }
        }

        public async Task RaiseAfterSaveFailedCompletedAsyncTriggers(Exception exception, CancellationToken cancellationToken)
        {
            var triggers = _triggerDiscoveryService.DiscoverTriggers<IAfterSaveFailedCompletedAsyncTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterSaveFailedCompletedAsync(exception, cancellationToken);
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
