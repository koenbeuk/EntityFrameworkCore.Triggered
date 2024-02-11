namespace EntityFrameworkCore.Triggered;

public interface ITriggerContext<out TEntity>
    where TEntity : class
{
    /// <summary>
    /// The type of change that caused this trigger to be raised 
    /// </summary>
    ChangeType ChangeType { get; }

    /// <summary>
    /// The entity that caused this trigger to be raised
    /// </summary>
    TEntity Entity { get; }

    /// <summary>
    /// This will retain the unmodified entity in case when the ChangeType is <c>Modified</c>
    /// </summary>
    TEntity? UnmodifiedEntity { get; }

    /// <summary>
    /// Gets or sets a key/value collection that can be used to share data within the scope of this Entity
    /// </summary>
    IDictionary<object, object> Items { get; }
}
