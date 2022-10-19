namespace EntityFrameworkCore.Triggered
{
    public interface IAfterSaveTrigger<TEntity>
        where TEntity : class
    {
        void AfterSave(ITriggerContext<TEntity> context);
    }
}
