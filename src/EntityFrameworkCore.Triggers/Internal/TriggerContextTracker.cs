using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggers.Internal
{
    public sealed class TriggerContextTracker 
    {
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
                    var existingChange = _discoveredChanges.Find(x => x.Entity == entry.Entity);
                    if (existingChange != null && !_recursionStrategy.CanRecurse(entry, changeType.Value, existingChange))
                    {
                        // skip this detection when we already detected it
                        continue;
                    }

                    var entityType = entry.Entity.GetType();
                    var changeContextType = typeof(TriggerContext<>).MakeGenericType(entityType);
                    var triggerContext = (ITriggerContextDescriptor)Activator.CreateInstance(changeContextType, new object[] { changeType.Value, entry });

                    _discoveredChanges.Add(triggerContext);

                    yield return triggerContext;
                }
            }
        }

        public IEnumerable<ITriggerContextDescriptor>? DiscoveredChanges => _discoveredChanges;
    }


}
