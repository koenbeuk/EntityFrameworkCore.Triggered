using System.Collections.Generic;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerContext<TEntity> : ITriggerContext<TEntity>
        where TEntity : class
    {
        readonly TEntity _entity;
        readonly ChangeType _type;
        readonly PropertyValues? _originalValues;
        readonly EntityBagStateManager _entityBagStateManager;

        TEntity? _unmodifiedEntity;

        public TriggerContext(object entity, PropertyValues? originalValues, ChangeType changeType, EntityBagStateManager entityBagStateManager)
        {
            _entity = (TEntity)entity;
            _originalValues = originalValues;
            _type = changeType;
            _entityBagStateManager = entityBagStateManager;
        }

        public ChangeType ChangeType => _type;
        public TEntity Entity => _entity;
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

        public IDictionary<object, object> Items => _entityBagStateManager.GetForEntity(_entity);
    }
}
