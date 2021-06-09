using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Extensions
{
    public class Trigger<TEntity> :
        IBeforeSaveTrigger<TEntity>,
        IAfterSaveTrigger<TEntity>,
        IAfterSaveFailedTrigger<TEntity>

        where TEntity : class
    {
        public virtual void BeforeSave(ITriggerContext<TEntity> context) { }
        public virtual void AfterSave(ITriggerContext<TEntity> context) { }
        public virtual void AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception) { }

        public virtual Task BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken) => Task.CompletedTask;
        public virtual Task AfterSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken) => Task.CompletedTask;
        public virtual Task AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken) => Task.CompletedTask;

        Task IBeforeSaveTrigger<TEntity>.BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            BeforeSave(context);
            return BeforeSave(context, cancellationToken);
        }

        Task IAfterSaveTrigger<TEntity>.AfterSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            AfterSave(context);
            return AfterSave(context, cancellationToken);
        }

        Task IAfterSaveFailedTrigger<TEntity>.AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken)
        {
            AfterSaveFailed(context, exception);
            return AfterSaveFailed(context, exception, cancellationToken);
        }
    }
}
