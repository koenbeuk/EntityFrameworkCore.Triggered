using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Internal
{
    public class CascadingTriggerContextDiscoveryStrategy : ITriggerContextDiscoveryStrategy
    {
        readonly static Action<ILogger, string, int, Exception?> _discoveryStarted = LoggerMessage.Define<string, int>(
            LogLevel.Debug,
            new EventId(1, "Discovered"),
            "Starting trigger discovery for {name} with a max cascade of {maxCascadingCycles}");

        readonly static Action<ILogger, int, string, int, int, Exception?> _changesDetected = LoggerMessage.Define<int, string, int, int>(
            LogLevel.Debug,
            new EventId(1, "Discovered"),
            "Discovered changes: {changes} for {name}. Iteration ({iteration}/{maxCascadingCycles})");

        readonly string _name;
        readonly bool _skipDetectedChanges;

        public CascadingTriggerContextDiscoveryStrategy(string name, bool skipDetectedChanges)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _skipDetectedChanges = skipDetectedChanges;
        }

        public IEnumerable<IEnumerable<TriggerContextDescriptor>> Discover(TriggerSessionConfiguration configuration, TriggerContextTracker tracker, ILogger logger)
        {
            var maxCascadingCycles = configuration.MaxCascadeCycles;
            _discoveryStarted(logger, _name, maxCascadingCycles, null);

            var iteration = 0;
            while (true)
            {
                if (iteration > maxCascadingCycles)
                {
                    throw new InvalidOperationException("MaxCascadingCycle was reached");
                }

                var changes = tracker.DiscoverChanges();

                // In case someone made a call to TriggerSession.DetectChanges, prior to calling RaiseBeforeSaveTriggers, we want to make sure that we include that discovery result in the first iteration
                if (iteration == 0 && !_skipDetectedChanges)
                {
                    changes = tracker.DiscoveredChanges!;
                }

                if (changes.Any())
                {
                    if (logger.IsEnabled(LogLevel.Debug))
                    {
                        changes = changes.ToList();
                        _changesDetected(logger, changes.Count(), _name, iteration, maxCascadingCycles, null);
                    }

                    yield return changes;
                }
                else
                {
                    break;
                }

                iteration++;
            }
        }
    }
}
