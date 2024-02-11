namespace EntityFrameworkCore.Triggered.Transactions;

public interface IBeforeRollbackTrigger<in TEntity>
    where TEntity : class
{
    void BeforeRollback(ITriggerContext<TEntity> context);
}
