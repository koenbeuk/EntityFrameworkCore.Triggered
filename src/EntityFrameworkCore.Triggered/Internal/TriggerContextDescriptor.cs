using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerContextDescriptor : ITriggerContextDescriptor
    {
        [ThreadStatic]
        static Dictionary<Type, Func<EntityEntry, ChangeType, object>>? _cachedTriggerContextFactories;

        private EntityEntry? _entityEntry;
        private ChangeType _changeType;

        public void Initialize(EntityEntry entityEntry, ChangeType changeType)
        {
            _entityEntry = entityEntry;
            _changeType = changeType;
        }

        public void Reset()
        {
            _entityEntry = null;
            _changeType = default;
        }

        public ChangeType ChangeType => _changeType;
        public object Entity => _entityEntry!.Entity;
        public Type EntityType => _entityEntry!.Entity.GetType();

        public object GetTriggerContext()
        {
            var entityEntry = _entityEntry;
            var changeType = _changeType;

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
