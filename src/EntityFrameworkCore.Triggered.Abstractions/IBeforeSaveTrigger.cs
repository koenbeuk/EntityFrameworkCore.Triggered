namespace EntityFrameworkCore.Triggered;

public interface IBeforeSaveTrigger<in TEntity>
    where TEntity : class
{
    void BeforeSave(ITriggerContext<TEntity> context);
}
