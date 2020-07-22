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
        readonly Type _genericTriggerType;
        readonly Func<object, TriggerAdapterBase> _triggerAdapterFactory;

        public RecursiveTriggerContextDiscoveryStrategy(string name, Type genericTriggerType, Func<object, TriggerAdapterBase> triggerAdapterFactory)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _genericTriggerType = genericTriggerType ?? throw new ArgumentNullException(nameof(genericTriggerType));
            _triggerAdapterFactory = triggerAdapterFactory ?? throw new ArgumentNullException(nameof(triggerAdapterFactory));
        }

        public IEnumerable<(TriggerAdapterBase triggerAdapter, ITriggerContextDescriptor triggerContextDescriptor)> Discover(TriggerOptions options, ITriggerRegistryService triggerRegistryService, TriggerContextTracker tracker, ILogger logger)
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

                    // In case someone made a call to TriggerSession.DetectChanges, prior to calling RaiseBeforeSaveTriggers, we want to make sure that we include that discovery result in the first iteration
                    if (iteration == 0 && tracker.DiscoveredChanges != null)
                    {
                        // In case there are more yet undiscovered changes, make another call
                        tracker.DiscoverChanges();
                        changes = tracker.DiscoveredChanges!;
                    }
                    else
                    {
                        changes = tracker.DiscoverChanges().ToList();
                    }

                    if (changes.Any())
                    {
                        logger.LogInformation("({iteration}/{maxRecursion}): Detected changes: {changes}", iteration, maxRecursion, changes.Count());

                        foreach (var triggerContextDescriptor in changes)
                        {
                            var triggers = triggerRegistryService
                                .GetRegistry(_genericTriggerType, _triggerAdapterFactory)
                                .DiscoverTriggers(triggerContextDescriptor.EntityType)
                                .ToList();

                            logger.LogDebug("Discovered {triggers} triggers for change of type {entityType}", triggers.Count(), triggerContextDescriptor.EntityType);

                            foreach (var trigger in triggers)
                            {
                                yield return (trigger, triggerContextDescriptor);
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
        }
    }
}
