using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public abstract class Trigger<TEntity> : IBeforeSaveTrigger<TEntity>, IAfterSaveTrigger<TEntity>
        where TEntity: class
    {
        public virtual Task BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public virtual Task AfterSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
