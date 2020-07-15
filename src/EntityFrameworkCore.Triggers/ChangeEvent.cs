using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggers
{
    public class ChangeEvent<TEntity> : IChangeEvent<TEntity> , IChangeEventDescriptor
        where TEntity: class
    {
        readonly EntityEntry _entityEntry;
        readonly Lazy<TEntity?> _unmodifiedEntityLazy;

        TEntity? CreateUnmodified()
        {
            if (Type == ChangeType.Added)
            {
                return null;
            }
            else
            {
                return (TEntity)_entityEntry.OriginalValues.ToObject();
            }
        }


        public ChangeEvent(ChangeType changeType, EntityEntry entityEntry)
        {
            Type = changeType;
            _entityEntry = entityEntry;
            _unmodifiedEntityLazy = new Lazy<TEntity?>(CreateUnmodified, false);
        }

        public ChangeType Type { get; }
        public TEntity Entity => (TEntity)_entityEntry.Entity;
        public TEntity? UnmodifiedEntity => _unmodifiedEntityLazy.Value;

        object IChangeEventDescriptor.Entity => Entity;
        Type IChangeEventDescriptor.EntityType => typeof(TEntity);
        object IChangeEventDescriptor.GetChangeEvent() => this;
    }
}
