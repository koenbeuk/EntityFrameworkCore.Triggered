namespace EntityFrameworkCore.Triggered
{
    public interface IAfterSaveFailedAsyncTrigger<TEntity>
        where TEntity : class
    {
        Task AfterSaveFailedAsync(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken);
    }


}
