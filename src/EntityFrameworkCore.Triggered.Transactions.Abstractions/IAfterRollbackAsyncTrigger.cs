namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IAfterRollbackAsyncTrigger<in TEntity>
        where TEntity : class
    {
        Task AfterRollbackAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
