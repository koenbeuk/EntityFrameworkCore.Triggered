using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerContextTracker 
    {
        [ThreadStatic]
        static Dictionary<Type, Func<EntityEntry, ChangeType, object>>? _cachedTriggerContextFactories;

        private ITriggerContextDescriptor CreateTriggerContext(Type entityType, EntityEntry entityEntry, ChangeType changeType)
        {
            if (_cachedTriggerContextFactories == null)
            {
                _cachedTriggerContextFactories = new Dictionary<Type, Func<EntityEntry, ChangeType, object>>();
            }

            if (!_cachedTriggerContextFactories.TryGetValue(entityType, out var triggerContextFactory))
            {
                triggerContextFactory = (Func<EntityEntry, ChangeType, object>)typeof(TriggerContextFactory<>).MakeGenericType(entityType)
                    .GetMethod(nameof(TriggerContextFactory<object>.Activate))
                    .CreateDelegate(typeof(Func<EntityEntry, ChangeType, object>));

                _cachedTriggerContextFactories.Add(entityType, triggerContextFactory);
            }

            return (ITriggerContextDescriptor)triggerContextFactory(entityEntry, changeType);
        }


        readonly ChangeTracker _changeTracker;
        readonly IRecursionStrategy _recursionStrategy;

        List<ITriggerContextDescriptor>? _discoveredChanges;

        public TriggerContextTracker(ChangeTracker changeTracker, IRecursionStrategy recursionStrategy)
        {
            _changeTracker = changeTracker;
            _recursionStrategy = recursionStrategy;
        }

        static ChangeType? ResolveChangeType(EntityEntry entry) => entry.State switch
        {
            EntityState.Added => ChangeType.Added,
            EntityState.Modified => ChangeType.Modified,
            EntityState.Deleted => ChangeType.Deleted,
            _ => null,
        };

        public IEnumerable<ITriggerContextDescriptor> DiscoverChanges()
        {
            if (_discoveredChanges == null)
            {
                _discoveredChanges = new List<ITriggerContextDescriptor>();
            }

            _changeTracker.DetectChanges();
            foreach (var entry in _changeTracker.Entries())
            {
                var changeType = ResolveChangeType(entry);
                if (changeType != null)
                {
                    var existingChanges = _discoveredChanges.Where(x => x.Entity == entry.Entity);
                    if (existingChanges.Any() && existingChanges.Any(existingChange => !_recursionStrategy.CanRecurse(entry, changeType.Value, existingChange)))
                    {
                        // skip this detection when we already detected it
                        continue;
                    }

                    var entityType = entry.Entity.GetType();
                    var triggerContext = CreateTriggerContext(entityType, entry, changeType.Value);

                    _discoveredChanges.Add(triggerContext);

                    yield return triggerContext;
                }
            }
        }

        public IEnumerable<ITriggerContextDescriptor>? DiscoveredChanges => _discoveredChanges;
    }
}
