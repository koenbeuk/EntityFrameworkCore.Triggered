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
        /// Disables recursion. Any changes made in <see cref="EntityFrameworkCore.Triggers.IBeforeSaveChangeEventHandler{TEntity}"/> will not raise additional events
        /// </summary>
        /// <remarks>
        /// No recursion is often not desired since it puts a soft restriction on <see cref="EntityFrameworkCore.Triggers.IBeforeSaveChangeEventHandler{TEntity}"/>.
        /// </remarks>
        None,
        /// <summary>
        /// (Default) Events are only raised once per Entity and <see cref="EntityFrameworkCore.Triggers.ChangeType"/>. 
        /// </summary>
        EntityAndType
    }
}
