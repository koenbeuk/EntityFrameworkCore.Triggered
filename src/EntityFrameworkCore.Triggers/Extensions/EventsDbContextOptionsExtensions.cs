using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Infrastructure;
using EntityFrameworkCore.Triggers.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Events-specific extension methods for <see cref="DbContextOptionsBuilder" />.
    /// </summary>
    public static class EventsDbContextOptionsExtensions
    {

        public static DbContextOptionsBuilder<TContext> UseEvents<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, Action<EventsContextOptionsBuilder>? configure)
            where TContext : DbContext => (DbContextOptionsBuilder<TContext>)UseEvents((DbContextOptionsBuilder)optionsBuilder, configure);

        public static DbContextOptionsBuilder UseEvents(this DbContextOptionsBuilder optionsBuilder, Action<EventsContextOptionsBuilder>? configure)
        {
            if (optionsBuilder is null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            var extension = optionsBuilder.Options.FindExtension<EventsOptionExtension>() ?? new EventsOptionExtension();
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            configure?.Invoke(new EventsContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

    }
}
