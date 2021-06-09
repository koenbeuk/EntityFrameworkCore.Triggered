using System;
using EntityFrameworkCore.Triggered.Infrastructure;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    ///     Triggers-specific extension methods for <see cref="DbContextOptionsBuilder" />.
    /// </summary>
    public static class TriggersDbContextOptionsExtensions
    {
        public static DbContextOptionsBuilder<TContext> UseTriggers<TContext>(this DbContextOptionsBuilder<TContext> optionsBuilder, Action<TriggersContextOptionsBuilder>? configure = null)
            where TContext : DbContext => (DbContextOptionsBuilder<TContext>)UseTriggers((DbContextOptionsBuilder)optionsBuilder, configure);

        public static DbContextOptionsBuilder UseTriggers(this DbContextOptionsBuilder optionsBuilder, Action<TriggersContextOptionsBuilder>? configure = null)
        {
            if (optionsBuilder is null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            var extension = optionsBuilder.Options.FindExtension<TriggersOptionExtension>() ?? new TriggersOptionExtension();
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            configure?.Invoke(new TriggersContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }
    }
}
