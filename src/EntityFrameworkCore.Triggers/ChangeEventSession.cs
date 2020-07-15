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
    public class ChangeEventSession : IChangeEventSession
    {
        readonly IChangeEventHandlerRegistryService _changeEventHandlerRegistryService;
        readonly ChangeEventTracker _tracker;
        readonly ILogger<ChangeEventSession> _logger;

        public ChangeEventSession(IChangeEventHandlerRegistryService changeEventHandlerRegistryService, ChangeEventTracker tracker, ILoggerFactory loggerFactory)
        {
            _changeEventHandlerRegistryService = changeEventHandlerRegistryService;
            _tracker = tracker;
            _logger = loggerFactory.CreateLogger<ChangeEventSession>();
        }

        public async Task RaiseBeforeSaveChangeEvents(CancellationToken cancellationToken)
        {
            var maxRecursion = 100; // todo

            _logger.LogDebug("Starting BeforeSave change event raising with a max recursion of {maxRecursion}", maxRecursion);

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

                    foreach (var changeEventDescriptor in changes)
                    {
                        var handlers = _changeEventHandlerRegistryService
                            .GetRegistry(typeof(IBeforeSaveChangeEventHandler<>), changeHandler => new BeforeSaveEventHandlerAdapter(changeHandler))
                            .DiscoverChangeHandlers(changeEventDescriptor.EntityType)
                            .ToList();

                        _logger.LogDebug("Discovered {handlers} handlers for change event of type {entityType}", handlers.Count(), changeEventDescriptor.EntityType);

                        foreach (var handler in handlers)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await handler.Execute(changeEventDescriptor.GetChangeEvent(), cancellationToken).ConfigureAwait(false);
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

        public async Task RaiseAfterSaveChangeEvents(CancellationToken cancellationToken = default)
        {
            var changes = _tracker.DiscoveredChanges ?? throw new InvalidOperationException("RaisBeforeSaveChangeEvents requires to be called for RaiseAfterSaveChangeEvents to work properly");

            _logger.LogInformation("AfterSave: Detected {changes} changes", changes.Count());

            foreach (var change in changes)
            {
                var handlers = _changeEventHandlerRegistryService
                    .GetRegistry(typeof(IAfterSaveChangeEventHandler<>), changeHandler => new AfterSaveEventHandlerAdapter(changeHandler))
                    .DiscoverChangeHandlers(change.EntityType)
                    .ToList();

                _logger.LogDebug("Discovered {handlers} handlers for change event of type {entityType}", handlers.Count(), change.EntityType);

                foreach (var handler in handlers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await handler.Execute(change.GetChangeEvent(), cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
