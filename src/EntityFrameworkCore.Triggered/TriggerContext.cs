using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerContext<TEntity> : ITriggerContext<TEntity>
        where TEntity : class
    {
        readonly ChangeType _type;
        readonly TEntity _entity;
        readonly TEntity? _unmodifiedEntity;


        public TriggerContext(object entity, PropertyValues? originalValues, ChangeType changeType)
        {
            _type = changeType;
            _entity = (TEntity)entity;

            if (originalValues != null && changeType != ChangeType.Added)
            {
                _unmodifiedEntity = (TEntity)originalValues.ToObject();
            }
        }

        public ChangeType ChangeType => _type;
        public TEntity Entity => _entity;
        public TEntity? UnmodifiedEntity => _unmodifiedEntity;
    }
}
