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
        readonly EntityEntry _entityEntry;
        readonly Lazy<TEntity?> _unmodifiedEntityLazy;

        TEntity? CreateUnmodified()
        {
            if (ChangeType == ChangeType.Added)
            {
                return null;
            }
            else
            {
                return (TEntity)_entityEntry.OriginalValues.ToObject();
            }
        }


        public TriggerContext(EntityEntry entityEntry, ChangeType changeType)
        {
            _type = changeType;
            _entityEntry = entityEntry;
            _unmodifiedEntityLazy = new Lazy<TEntity?>(CreateUnmodified, false);
        }

        public ChangeType ChangeType => _type;
        public TEntity Entity => (TEntity)_entityEntry.Entity;
        public TEntity? UnmodifiedEntity => _unmodifiedEntityLazy.Value;
    }
}
