namespace EntityFrameworkCore.Triggered.Infrastructure
{
    public enum CascadeBehavior
    {
        /// <summary>
        /// Disables cascading. Any changes made in <see cref="EntityFrameworkCore.Triggered.IBeforeSaveTrigger{TEntity}"/> will not raise additional triggers
        /// </summary>
        /// <remarks>
        /// No cascading is often not desired since it puts a soft restriction on <see cref="EntityFrameworkCore.Triggered.IBeforeSaveTrigger{TEntity}"/>.
        /// </remarks>
        None,
        /// <summary>
        /// (Default) Triggers are only raised once per entity and change type <see cref="EntityFrameworkCore.Triggered.ChangeType"/>. 
        /// </summary>
        EntityAndType
    }
}
