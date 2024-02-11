namespace EntityFrameworkCore.Triggered.Extensions
{
    public class Trigger<TEntity> :
        IBeforeSaveTrigger<TEntity>,
        IBeforeSaveAsyncTrigger<TEntity>,
        IAfterSaveTrigger<TEntity>,
        IAfterSaveAsyncTrigger<TEntity>,
        IAfterSaveFailedTrigger<TEntity>,
        IAfterSaveFailedAsyncTrigger<TEntity>

        where TEntity : class
    {
        public virtual void BeforeSave(ITriggerContext<TEntity> context) { }
        public virtual void AfterSave(ITriggerContext<TEntity> context) { }
        public virtual void AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception) { }
        public virtual Task BeforeSaveAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken) => Task.CompletedTask;
        public virtual Task AfterSaveAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken) => Task.CompletedTask;
        public virtual Task AfterSaveFailedAsync(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
