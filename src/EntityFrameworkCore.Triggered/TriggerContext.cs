using System.Collections.Generic;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerContext<TEntity>(EntityEntry entityEntry, PropertyValues? originalValues, ChangeType changeType, EntityBagStateManager entityBagStateManager) : ITriggerContext<TEntity>
        where TEntity : class
    {
        readonly EntityEntry _entityEntry = entityEntry;
        readonly ChangeType _type = changeType;
        readonly PropertyValues? _originalValues = originalValues;
        readonly EntityBagStateManager _entityBagStateManager = entityBagStateManager;

        TEntity? _unmodifiedEntity;

        public ChangeType ChangeType => _type;
        public TEntity Entity => (TEntity)_entityEntry.Entity;
        public TEntity? UnmodifiedEntity
        {
            get
            {
                if (_type == ChangeType.Added)
                {
                    return null;
                }
                else
                {
                    if (_unmodifiedEntity == null && _originalValues != null)
                    {
                        _unmodifiedEntity = (TEntity)_originalValues.ToObject();
                    }

                    return _unmodifiedEntity;
                }
            }
        }

        public IDictionary<object, object> Items => _entityBagStateManager.GetForEntity(_entityEntry.Entity);

        public EntityEntry<TEntity> Entry => (EntityEntry<TEntity>)_entityEntry;
    }
}
