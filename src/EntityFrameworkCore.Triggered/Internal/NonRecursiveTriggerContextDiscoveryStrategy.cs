using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Internal
{
    public class NonRecursiveTriggerContextDiscoveryStrategy : ITriggerContextDiscoveryStrategy
    {
        readonly string _name;

        public NonRecursiveTriggerContextDiscoveryStrategy(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public IEnumerable<ITriggerContextDescriptor> Discover(TriggerOptions options, TriggerContextTracker tracker, ILogger logger)
        {
            using (logger.BeginScope(" {triggerType} triggers", _name))
            {
                var changes = tracker.DiscoveredChanges ?? throw new InvalidOperationException("Trigger discovery process has not yet started. Please ensure that TriggerSession.DiscoverChanges() or TriggerSession.RaiseBeforeSaveTriggers() has been called");

                logger.LogInformation("Detected changes: {changes}", changes.Count());

                return changes;
            }
        }
    }
}
