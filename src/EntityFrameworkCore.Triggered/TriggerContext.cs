using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerContext<TEntity> : ITriggerContext<TEntity>
        where TEntity: class
    {
        readonly ChangeType _type;
        readonly TEntity _entity;
        readonly TEntity? _unmodifiedEntity;


        public TriggerContext(EntityEntry entityEntry, ChangeType changeType)
        {
            _type = changeType;
            _entity = (TEntity)entityEntry.Entity;
            
            if (changeType != ChangeType.Added)
            {
                _unmodifiedEntity = (TEntity)entityEntry.OriginalValues.ToObject();
            }
        }

        public ChangeType ChangeType => _type;
        public TEntity Entity => _entity;
        public TEntity? UnmodifiedEntity => _unmodifiedEntity;
    }
}
