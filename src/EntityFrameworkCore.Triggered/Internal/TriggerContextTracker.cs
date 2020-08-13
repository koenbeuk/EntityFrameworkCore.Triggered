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
using Microsoft.Extensions.ObjectPool;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerContextTracker
    {
        readonly ChangeTracker _changeTracker;
        readonly IRecursionStrategy _recursionStrategy;

        List<TriggerContextDescriptor>? _discoveredChanges;

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

        public IEnumerable<TriggerContextDescriptor> DiscoverChanges()
        {
            int startIndex;

            if (_discoveredChanges == null)
            {
                _discoveredChanges = new List<TriggerContextDescriptor>();
                startIndex = 0;
            }
            else
            {
                startIndex = _discoveredChanges.Count;
            }

            _changeTracker.DetectChanges();
            var entries = _changeTracker.Entries();
            foreach (var entry in entries)
            {
                var changeType = ResolveChangeType(entry);
                if (changeType != null)
                {
                    if (startIndex > 0)
                    {
                        var canRecurse = true;

                        foreach (var discoveredChange in _discoveredChanges)
                        {
                            if (discoveredChange.Entity == entry.Entity)
                            {
                                canRecurse = _recursionStrategy.CanRecurse(entry, changeType.Value, discoveredChange);

                                if (!canRecurse)
                                {
                                    break;
                                }
                            }
                        }

                        if (!canRecurse)
                        {
                            continue;
                        }
                    }

                    var triggerContextDescriptor = new TriggerContextDescriptor(entry, changeType.Value);

                    _discoveredChanges.Add(triggerContextDescriptor!);
                }
            }

            if (startIndex == 0)
            {
                return _discoveredChanges;
            }
            else
            {
                return _discoveredChanges.Skip(startIndex);
            }
        }

        public IEnumerable<TriggerContextDescriptor>? DiscoveredChanges => _discoveredChanges;
    }
}
