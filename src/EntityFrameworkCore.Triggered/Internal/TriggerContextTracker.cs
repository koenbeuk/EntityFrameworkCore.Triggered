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
    public sealed class TriggerContextTracker : IDisposable
    {
        static readonly ObjectPool<TriggerContextDescriptor> _triggerContextDescriptorPool = new DefaultObjectPool<TriggerContextDescriptor>(new TriggerContextDescriptorPooledPolicy());

        private ITriggerContextDescriptor CreateTriggerContextDescriptor(EntityEntry entityEntry, ChangeType changeType)
        {
            var descriptor = _triggerContextDescriptorPool.Get();
            descriptor.Initialize(entityEntry, changeType);

            return descriptor;
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

                    var triggerContextDescriptor = CreateTriggerContextDescriptor(entry, changeType.Value);

                    _discoveredChanges.Add(triggerContextDescriptor);

                    yield return triggerContextDescriptor;
                }
            }
        }

        public IEnumerable<ITriggerContextDescriptor>? DiscoveredChanges => _discoveredChanges;

        public void Dispose()
        {
            var discoveredChanges = _discoveredChanges;

            if (discoveredChanges != null)
            {
                foreach (var triggerContextDescriptor in discoveredChanges)
                {
                    _triggerContextDescriptorPool.Return((TriggerContextDescriptor)triggerContextDescriptor);
                }
                   
                _discoveredChanges = null;
            }
        }
    }
}
