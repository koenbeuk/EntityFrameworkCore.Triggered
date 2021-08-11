using System.Collections.Generic;

namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerContext<out TEntity>
        where TEntity : class
    {
        ChangeType ChangeType { get; }

        TEntity Entity { get; }

        TEntity? UnmodifiedEntity { get; }

        /// <summary>
        /// Gets or sets a key/value collection that can be used to share data within the scope of this Entity
        /// </summary>
        IDictionary<object, object> Items { get; }
    }
}
