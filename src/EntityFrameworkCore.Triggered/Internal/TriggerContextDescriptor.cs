using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal
{
    public readonly struct TriggerContextDescriptor
    {
        [ThreadStatic]
        static Dictionary<Type, Func<object, PropertyValues?, ChangeType, object>>? _cachedTriggerContextFactories;

        readonly EntityEntry _entityEntry;
        readonly ChangeType _changeType;
        readonly PropertyValues? _originalValues;

        public TriggerContextDescriptor(EntityEntry entityEntry, ChangeType changeType)
        {
            _entityEntry = entityEntry;
            _changeType = changeType;
            _originalValues = entityEntry.OriginalValues.Clone();
        }

        public ChangeType ChangeType => _changeType;
        public object Entity => _entityEntry!.Entity;
        public Type EntityType => _entityEntry!.Entity.GetType();

        public object GetTriggerContext()
        {
            var entityEntry = _entityEntry;
            var changeType = _changeType;
            var originalValues = _originalValues;

            if (entityEntry == null)
            {
                throw new InvalidOperationException("No initialized");
            }

            var entityType = entityEntry.Entity.GetType();

            if (_cachedTriggerContextFactories == null)
            {
                _cachedTriggerContextFactories = new Dictionary<Type, Func<object, PropertyValues?, ChangeType, object>>();
            }

            if (!_cachedTriggerContextFactories.TryGetValue(entityType, out var triggerContextFactory))
            {
                triggerContextFactory = (Func<object, PropertyValues?, ChangeType, object>)typeof(TriggerContextFactory<>).MakeGenericType(entityType)
                    .GetMethod(nameof(TriggerContextFactory<object>.Activate))
                    .CreateDelegate(typeof(Func<object, PropertyValues?, ChangeType, object>));

                _cachedTriggerContextFactories.Add(entityType, triggerContextFactory);
            }

            return triggerContextFactory(entityEntry.Entity, originalValues, changeType);
        }
    }
}
