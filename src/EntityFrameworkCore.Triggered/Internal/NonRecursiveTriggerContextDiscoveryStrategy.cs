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
        readonly Type _genericTriggerType;
        readonly Func<object, TriggerAdapterBase> _triggerAdapterFactory;

        public NonRecursiveTriggerContextDiscoveryStrategy(string name, Type genericTriggerType, Func<object, TriggerAdapterBase> triggerAdapterFactory)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _genericTriggerType = genericTriggerType ?? throw new ArgumentNullException(nameof(genericTriggerType));
            _triggerAdapterFactory = triggerAdapterFactory ?? throw new ArgumentNullException(nameof(triggerAdapterFactory));
        }

        public IEnumerable<(TriggerAdapterBase triggerAdapter, ITriggerContextDescriptor triggerContextDescriptor)> Discover(TriggerOptions options, ITriggerRegistryService triggerRegistryService, TriggerContextTracker tracker, ILogger logger)
        {
            using (logger.BeginScope(" {triggerType} triggers", _name))
            {
                var changes = tracker.DiscoveredChanges ?? throw new InvalidOperationException("Trigger discovery process has not yet started. Please ensure that TriggerSession.DiscoverChanges() or TriggerSession.RaiseBeforeSaveTriggers() has been called");

                logger.LogInformation("Detected changes: {changes}", changes.Count());

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
        }
    }
}
