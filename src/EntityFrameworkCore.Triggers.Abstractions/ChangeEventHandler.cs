using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggers
{
    public abstract class ChangeEventHandler<TEntity> : IBeforeSaveChangeEventHandler<TEntity>, IAfterSaveChangeEventHandler<TEntity>
        where TEntity: class
    {
        public virtual Task BeforeSave(IChangeEvent<TEntity> @event, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public virtual Task AfterSave(IChangeEvent<TEntity> @event, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
