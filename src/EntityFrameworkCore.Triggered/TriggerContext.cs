using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerContext<TEntity> : ITriggerContext<TEntity>
        where TEntity : class
    {
        readonly ChangeType _type;
        readonly TEntity _entity;
        readonly PropertyValues? _originalValues;

        TEntity? _unmodifiedEntity;


        public TriggerContext(object entity, PropertyValues? originalValues, ChangeType changeType)
        {
            _type = changeType;
            _entity = (TEntity)entity;
            _originalValues = originalValues;
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
    }
}
