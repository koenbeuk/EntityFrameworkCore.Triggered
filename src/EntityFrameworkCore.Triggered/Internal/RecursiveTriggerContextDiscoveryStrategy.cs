using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Internal
{
    public class RecursiveTriggerContextDiscoveryStrategy : ITriggerContextDiscoveryStrategy
    {
        readonly string _name;

        public RecursiveTriggerContextDiscoveryStrategy(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public IEnumerable<ITriggerContextDescriptor> Discover(TriggerOptions options, TriggerContextTracker tracker, ILogger logger)
        {
            using (logger.BeginScope("Discovering {triggerType} triggers", _name))
            {
                var maxRecursion = options.MaxRecursion;

                logger.LogDebug("Starting trigger discovery with a max recursion of {maxRecursion}", maxRecursion);

                var iteration = 0;
                while (true)
                {
                    if (iteration > maxRecursion)
                    {
                        throw new InvalidOperationException("MaxRecursion was reached");
                    }


                    IEnumerable<ITriggerContextDescriptor> changes;

                    changes = tracker.DiscoverChanges().ToList();
                    
                    // In case someone made a call to TriggerSession.DetectChanges, prior to calling RaiseBeforeSaveTriggers, we want to make sure that we include that discovery result in the first iteration
                    if (iteration == 0)
                    {
                        changes = tracker.DiscoveredChanges!;
                    }
                    else
                    {
                        changes = tracker.DiscoverChanges().ToList();
                    }

                    if (changes.Any())
                    {
                        logger.LogInformation("({iteration}/{maxRecursion}): Detected changes: {changes}", iteration, maxRecursion, changes.Count());

                        foreach (var change in changes)
                        {
                            yield return change;
                        }
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
}
