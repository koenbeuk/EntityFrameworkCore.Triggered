using EntityFrameworkCore.Triggers.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggers
{
    public class TriggerSession : ITriggerSession
    {
        readonly TriggerOptions _options;
        readonly ITriggerRegistryService _triggerRegistryService;
        readonly TriggerContextTracker _tracker;
        readonly ILogger<TriggerSession> _logger;

        public TriggerSession(TriggerOptions options, ITriggerRegistryService triggerRegistryService, TriggerContextTracker tracker, ILoggerFactory loggerFactory)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _triggerRegistryService = triggerRegistryService ?? throw new ArgumentNullException(nameof(TriggerRegistry));
            _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
            _logger = loggerFactory.CreateLogger<TriggerSession>();
        }

        public async Task RaiseBeforeSaveTriggers(CancellationToken cancellationToken)
        {
            var maxRecursion = _options.MaxRecursion;

            _logger.LogDebug("Starting BeforeSave triggers raising with a max recursion of {maxRecursion}", maxRecursion);

            var iteration = 0;
            while (true)
            {
                if (iteration > maxRecursion)
                {
                    throw new InvalidOperationException("MaxRecursion was reached");
                }

                var changes = _tracker.DiscoverChanges().ToList();

                if (changes.Any())
                {
                    _logger.LogInformation("BeforeSave: ({iteration}/{maxRecursion}): Detected {changes} changes", iteration, maxRecursion, changes.Count);

                    foreach (var triggerContextDescriptor in changes)
                    {
                        var triggers = _triggerRegistryService
                            .GetRegistry(typeof(IBeforeSaveTrigger<>), changeHandler => new BeforeSaveTriggerAdapter(changeHandler))
                            .DiscoverChangeHandlers(triggerContextDescriptor.EntityType)
                            .ToList();

                        _logger.LogDebug("Discovered {triggers} triggers for change of type {entityType}", triggers.Count(), triggerContextDescriptor.EntityType);

                        foreach (var handler in triggers)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await handler.Execute(triggerContextDescriptor.GetTriggerContext(), cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
                else
                {
                    break;
                }

                iteration++;
            }
        }

        public async Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default)
        {
            var changes = _tracker.DiscoveredChanges ?? throw new InvalidOperationException("RaisBeforeSaveTriggers requires to be called for RaiseAfterSaveTriggers to work properly");

            _logger.LogInformation("AfterSave: Detected {changes} changes", changes.Count());

            foreach (var change in changes)
            {
                var triggers = _triggerRegistryService
                    .GetRegistry(typeof(IAfterSaveTrigger<>), changeHandler => new AfterSaveTriggerAdapter(changeHandler))
                    .DiscoverChangeHandlers(change.EntityType)
                    .ToList();

                _logger.LogDebug("Discovered {triggers} triggers for change of type {entityType}", triggers.Count(), change.EntityType);

                foreach (var trigger in triggers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await trigger.Execute(change.GetTriggerContext(), cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
