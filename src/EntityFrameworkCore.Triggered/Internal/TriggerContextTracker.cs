using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

            _changeTracker.DetectChanges();
            var entries = _changeTracker.Entries();

            if (_discoveredChanges == null)
            {
                _discoveredChanges = new List<TriggerContextDescriptor>(entries.Count());
                startIndex = 0;
            }
            else
            {
                startIndex = _discoveredChanges.Count;
            }

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

        public void CaptureChanges()
        {
            if (_discoveredChanges != null && _discoveredChanges.Count > 0)
            {
                List<TriggerContextDescriptor>? ignoreCandidates = null;

                foreach (var discoveredChange in _discoveredChanges)
                {
                    var currentEntityEntry = _changeTracker.Context.Entry(discoveredChange.Entity);
                    var changeType = ResolveChangeType(currentEntityEntry);

                    if (changeType != discoveredChange.ChangeType)
                    {
                        if (ignoreCandidates == null)
                        {
                            ignoreCandidates = new List<TriggerContextDescriptor>();
                        }

                        ignoreCandidates.Add(discoveredChange);
                    }
                }

                if (ignoreCandidates != null)
                {
                    foreach (var ignoreCandidate in ignoreCandidates)
                    {
                        _discoveredChanges.Remove(ignoreCandidate);
                    }
                }
            }
        }

        public IEnumerable<TriggerContextDescriptor>? DiscoveredChanges => _discoveredChanges;
    }
}
