using System;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTriggeredDbContext<TContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            var serviceCollections = serviceCollection.AddDbContext<TContext>(options => {
                optionsAction?.Invoke(options);
                options.UseTriggers();
            }, contextLifetime, optionsLifetime);


            return serviceCollection;
        }

        public static IServiceCollection AddTriggeredDbContextPool<TContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, int poolSize = 128) where TContext : DbContext
        {

            serviceCollection.AddDbContextPool<TContext>(options => {
                optionsAction?.Invoke(options);
                options.UseTriggers();
            }, poolSize);


            return serviceCollection;
        }

    }
}
