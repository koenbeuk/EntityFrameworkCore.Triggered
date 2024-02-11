namespace EntityFrameworkCore.Triggered;

public interface IAfterSaveAsyncTrigger<TEntity>
    where TEntity : class
{
    Task AfterSaveAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
}
