using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Lyfecycles;

namespace EntityFrameworkCore.Triggered.Extensions
{
    public class Trigger<TEntity> :
        IBeforeSaveStartingTrigger,
        IBeforeSaveTrigger<TEntity>,
        IBeforeSaveCompletedTrigger,
        IAfterSaveStartingTrigger,
        IAfterSaveTrigger<TEntity>,
        IAfterSaveCompletedTrigger,
        IAfterSaveFailedStartingTrigger,
        IAfterSaveFailedTrigger<TEntity>,
        IAfterSaveFailedCompletedTrigger

        where TEntity : class
    {
        public virtual void BeforeSaveStarting() { }
        public virtual void BeforeSave(ITriggerContext<TEntity> context) { }
        public virtual void BeforeSaveCompleted() { }
        public virtual void AfterSaveStarting() { }
        public virtual void AfterSave(ITriggerContext<TEntity> context) { }
        public virtual void AfterSaveCompleted() { }
        public virtual void AfterSaveFailedStarting(Exception exception) { }
        public virtual void AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception) { }
        public virtual void AfterSaveFailedCompleted(Exception exception) { }

        public virtual Task BeforeSaveStarting(CancellationToken cancellationToken)
        {
            BeforeSaveStarting();
            return Task.CompletedTask;
        }

        public virtual Task BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            BeforeSave(context);
            return Task.CompletedTask;
        }

        public virtual Task BeforeSaveCompleted(CancellationToken cancellationToken)
        {
            BeforeSaveCompleted();
            return Task.CompletedTask;
        }

        public virtual Task AfterSaveStarting(CancellationToken cancellationToken)
        {
            AfterSaveStarting();
            return Task.CompletedTask;
        }

        public virtual Task AfterSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            AfterSave(context);
            return Task.CompletedTask;
        }

        public virtual Task AfterSaveCompleted(CancellationToken cancellationToken)
        {
            AfterSaveCompleted();
            return Task.CompletedTask;
        }

        public virtual Task AfterSaveFailedStarting(Exception exception, CancellationToken cancellationToken)
        {
            AfterSaveFailedStarting(exception);
            return Task.CompletedTask;
        } 

        public virtual Task AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken)
        {
            AfterSaveFailed(context, exception);
            return Task.CompletedTask;
        }

        public virtual Task AfterSaveFailedCompleted(Exception exception, CancellationToken cancellationToken)
        {
            AfterSaveFailedCompleted(exception);
            return Task.CompletedTask;
        }
    }
}
