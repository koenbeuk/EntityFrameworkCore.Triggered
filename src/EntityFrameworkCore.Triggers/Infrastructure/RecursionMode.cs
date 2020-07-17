using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggers.Infrastructure
{
    public enum RecursionMode
    {
        /// <summary>
        /// Disables recursion. Any changes made in <see cref="EntityFrameworkCore.Triggers.IBeforeSaveTrigger{TEntity}"/> will not raise additional triggers
        /// </summary>
        /// <remarks>
        /// No recursion is often not desired since it puts a soft restriction on <see cref="EntityFrameworkCore.Triggers.IBeforeSaveTrigger{TEntity}"/>.
        /// </remarks>
        None,
        /// <summary>
        /// (Default) Triggers are only raised once per entity and change type <see cref="EntityFrameworkCore.Triggers.ChangeType"/>. 
        /// </summary>
        EntityAndType
    }
}
