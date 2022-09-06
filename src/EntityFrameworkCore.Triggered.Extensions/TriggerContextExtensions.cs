using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace EntityFrameworkCore.Triggered
{
    public static class TriggerContextExtensions
    {
        /// <summary>
        /// Get the DbContext EntityEntry for the Entity
        /// </summary>
        /// <returns>The EntityEntry associated with the DbContext</returns>
        /// <exception cref="InvalidOperationException">Throws when the context is not of type <see cref="TriggerContext{TEntity}" /> </exception>
        public static EntityEntry<TEntity> GetEntry<TEntity>(this ITriggerContext<TEntity> context)
            where TEntity : class
        {
            if (context is not TriggerContext<TEntity> typedContext)
                throw new InvalidOperationException("GetEntry requires ITriggerContext<T> to be of type TriggerContext<T>");

            return typedContext.Entry;
        }

        /// <summary>
        /// Get the DbContext associated with this Entity
        /// </summary>
        /// <returns>The DbContext that fired this trigger</returns>
        /// <exception cref="InvalidOperationException">Throws when the context is not of type <see cref="TriggerContext{TEntity}" /> </exception>
        public static DbContext GetDbContext<TEntity>(this ITriggerContext<TEntity> context)
            where TEntity : class
        {
            if (context is not TriggerContext<TEntity> typedContext)
                throw new InvalidOperationException("GetDbContext requires ITriggerContext<T> to be of type TriggerContext<T>");

            return typedContext.Entry.Context;
        }
    }
}
