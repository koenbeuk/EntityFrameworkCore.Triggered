using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal
{
    public readonly struct TriggerContextDescriptor
    {
        [ThreadStatic]
        static Dictionary<Type, Func<EntityEntry, ChangeType, object>>? _cachedTriggerContextFactories;

        readonly EntityEntry _entityEntry;
        readonly ChangeType _changeType;

        public TriggerContextDescriptor(EntityEntry entityEntry, ChangeType changeType)
        {
            _entityEntry = entityEntry;
            _changeType = changeType;
            
        }

        public ChangeType ChangeType => _changeType;
        public object Entity => _entityEntry!.Entity;
        public Type EntityType => _entityEntry!.Entity.GetType();

        public object GetTriggerContext()
        {
            var entityEntry = _entityEntry;
            var changeType = ChangeType;

            if (entityEntry == null)
            {
                throw new InvalidOperationException("No initialized");
            }

            var entityType = entityEntry.Entity.GetType();

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

            return triggerContextFactory(entityEntry, changeType);
        }
    }
}
