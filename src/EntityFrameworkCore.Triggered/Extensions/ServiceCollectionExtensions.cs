using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        private static object CreateContextFactory(IServiceProvider serviceProvider, Type contextType)
        {
            var instance = ActivatorUtilities.CreateInstance(serviceProvider, contextType);

            if (instance is DbContext dbContextInstance)
            {
                var applicationTriggerServiceProviderAccessor = dbContextInstance.GetService<ApplicationTriggerServiceProviderAccessor>();
                if (applicationTriggerServiceProviderAccessor != null)
                {
                    applicationTriggerServiceProviderAccessor.SetTriggerServiceProvider(serviceProvider);
                }
            }

            return instance;
        }

        public static IServiceCollection AddTriggeredDbContext<TContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            serviceCollection.TryAdd(ServiceDescriptor.Describe(
                serviceType: typeof(TContext),
                implementationFactory: serviceProvider => CreateContextFactory(serviceProvider, typeof(TContext)),
                lifetime: contextLifetime));

            serviceCollection.AddDbContext<TContext>(options => {
                optionsAction?.Invoke(options);
                options.UseTriggers();
            }, contextLifetime, optionsLifetime);

            return serviceCollection;
        }
    }
}
