using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal;

public readonly struct TriggerContextDescriptor(EntityEntry entityEntry, ChangeType changeType)
{
    readonly static ConcurrentDictionary<Type, Func<EntityEntry, PropertyValues?, ChangeType, EntityBagStateManager, object>> _cachedTriggerContextFactories = new();

    readonly EntityEntry _entityEntry = entityEntry;
    readonly ChangeType _changeType = changeType;
    readonly PropertyValues? _originalValues = entityEntry.OriginalValues.Clone();

    public ChangeType ChangeType => _changeType;
    public object Entity => _entityEntry!.Entity;
    public Type EntityType => _entityEntry!.Entity.GetType();

    public object GetTriggerContext(EntityBagStateManager entityBagStateManager)
    {
        var entityEntry = _entityEntry;
        var changeType = _changeType;
        var originalValues = _originalValues;

        if (entityEntry == null)
        {
            throw new InvalidOperationException("No initialized");
        }

        var entityType = entityEntry.Entity.GetType();

        var triggerContextFactory = _cachedTriggerContextFactories.GetOrAdd(entityType, entityType =>
            (Func<EntityEntry, PropertyValues?, ChangeType, EntityBagStateManager, object>)typeof(TriggerContextFactory<>).MakeGenericType(entityType)
                !.GetMethod(nameof(TriggerContextFactory<object>.Activate))
                !.CreateDelegate(typeof(Func<EntityEntry, PropertyValues?, ChangeType, EntityBagStateManager, object>)));

        return triggerContextFactory(entityEntry, originalValues, changeType, entityBagStateManager);
    }
}
