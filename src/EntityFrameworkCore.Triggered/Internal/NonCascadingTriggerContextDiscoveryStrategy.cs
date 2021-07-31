using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Internal
{
    public class NonCascadingTriggerContextDiscoveryStrategy : ITriggerContextDiscoveryStrategy
    {
        readonly static Action<ILogger, int, string, Exception?> _changesDetected = LoggerMessage.Define<int, string>(
            LogLevel.Debug,
            new EventId(1, "Discovered"),
            "Discovered changes: {changes} for {name}");

        readonly string _name;

        public NonCascadingTriggerContextDiscoveryStrategy(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public IEnumerable<IEnumerable<TriggerContextDescriptor>> Discover(TriggerSessionConfiguration configuration, TriggerContextTracker tracker, ILogger logger)
        {
            var changes = tracker.DiscoveredChanges ?? throw new InvalidOperationException("Trigger discovery process has not yet started. Please ensure that TriggerSession.DiscoverChanges() or TriggerSession.RaiseBeforeSaveTriggers() has been called");

            if (logger.IsEnabled(LogLevel.Debug))
            {
                _changesDetected(logger, changes.Count(), _name, null);
            }

            return Enumerable.Repeat(changes, 1);
        }
    }
}
