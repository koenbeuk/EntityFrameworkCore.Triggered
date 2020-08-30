namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerContext<out TEntity>
        where TEntity : class
    {
        ChangeType ChangeType { get; }

        TEntity Entity { get; }

        TEntity? UnmodifiedEntity { get; }
    }
}
