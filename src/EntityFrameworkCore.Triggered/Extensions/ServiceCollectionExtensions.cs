using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
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

        private static object SetApplicationTriggerServiceProviderAccessor(object instance, IServiceProvider serviceProvider)
        {
            if (instance is DbContext dbContext)
            {
                var applicationTriggerServiceProviderAccessor = dbContext.GetService<ApplicationTriggerServiceProviderAccessor>();
                if (applicationTriggerServiceProviderAccessor != null)
                {
                    applicationTriggerServiceProviderAccessor.SetTriggerServiceProvider(serviceProvider);
                }
            }

            return instance;
        }

        public static IServiceCollection AddTriggeredDbContext<TContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) 
            where TContext : DbContext
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

        public static IServiceCollection AddTriggeredDbContextPool<TContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, int poolSize = 128) 
            where TContext : DbContext
        {
            serviceCollection.AddDbContextPool<TContext>(options => {
                optionsAction?.Invoke(options);
                options.UseTriggers();
            }, poolSize);

            var serviceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(TContext));
            if (serviceDescriptor?.ImplementationFactory != null)
            {
                serviceCollection.Replace(ServiceDescriptor.Describe(
                    serviceType: typeof(TContext),
                    implementationFactory: serviceProvider => SetApplicationTriggerServiceProviderAccessor(serviceDescriptor.ImplementationFactory(serviceProvider), serviceProvider),
                    lifetime: ServiceLifetime.Scoped
                ));
            }

            return serviceCollection;
        }

#if EFCORETRIGGERED2
        public static IServiceCollection AddTriggeredDbContextFactory<TContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
        {
            serviceCollection.AddDbContextFactory<TContext>(options => {
                optionsAction?.Invoke(options);
                options.UseTriggers();
            }, lifetime);

            var serviceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IDbContextFactory<TContext>));

            
            if (serviceDescriptor?.ImplementationType != null)
            {
                var triggeredFactoryType = typeof(TriggeredDbContextFactory<,>).MakeGenericType(typeof(TContext), serviceDescriptor.ImplementationType);

                serviceCollection.TryAdd(ServiceDescriptor.Describe(
                    serviceType: serviceDescriptor.ImplementationType,
                    implementationType: serviceDescriptor.ImplementationType,
                    lifetime: serviceDescriptor.Lifetime
                ));

                serviceCollection.Replace(ServiceDescriptor.Describe(
                    serviceType: typeof(IDbContextFactory<TContext>),
                    implementationFactory: serviceProvider => ActivatorUtilities.CreateInstance(serviceProvider, triggeredFactoryType, serviceProvider.GetService(serviceDescriptor.ImplementationType), serviceProvider),
                    lifetime: ServiceLifetime.Scoped
                ));
            }
            
            return serviceCollection;
        }

        public static IServiceCollection AddTriggeredDbContextFactory<TContext, TFactory>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
            where TFactory : IDbContextFactory<TContext>
        {
            serviceCollection.AddDbContextFactory<TContext>(options => {
                optionsAction?.Invoke(options);
                options.UseTriggers();
            }, lifetime);

            var serviceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IDbContextFactory<TContext>));

            if (serviceDescriptor?.ImplementationType != null)
            {
                var triggeredFactoryType = typeof(TriggeredDbContextFactory<,>).MakeGenericType(typeof(TContext), serviceDescriptor.ImplementationType);

                serviceCollection.TryAdd(ServiceDescriptor.Describe(
                    serviceType: serviceDescriptor.ImplementationType,
                    implementationType: serviceDescriptor.ImplementationType,
                    lifetime: serviceDescriptor.Lifetime
                ));

                serviceCollection.Replace(ServiceDescriptor.Describe(
                    serviceType: typeof(IDbContextFactory<TContext>),
                    implementationFactory: serviceProvider => ActivatorUtilities.CreateInstance(serviceProvider, triggeredFactoryType, serviceProvider.GetService(serviceDescriptor.ImplementationType), serviceProvider),
                    lifetime: ServiceLifetime.Scoped
                ));
            }

            return serviceCollection;
        }

        public static IServiceCollection AddTriggeredPooledDbContextFactory<TContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, int poolSize = 128)
            where TContext : DbContext
        {
            serviceCollection.AddPooledDbContextFactory<TContext>(options => {
                optionsAction?.Invoke(options);
                options.UseTriggers();
            }, poolSize);

            var serviceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IDbContextFactory<TContext>));

            if (serviceDescriptor?.ImplementationType != null)
            {
                var triggeredFactoryType = typeof(TriggeredDbContextFactory<,>).MakeGenericType(typeof(TContext), serviceDescriptor.ImplementationType);

                serviceCollection.TryAdd(ServiceDescriptor.Describe(
                    serviceType: serviceDescriptor.ImplementationType,
                    implementationType: serviceDescriptor.ImplementationType,
                    lifetime: serviceDescriptor.Lifetime
                ));

                serviceCollection.Replace(ServiceDescriptor.Describe(
                    serviceType: typeof(IDbContextFactory<TContext>),
                    implementationFactory: serviceProvider => ActivatorUtilities.CreateInstance(serviceProvider, triggeredFactoryType, serviceProvider.GetService(serviceDescriptor.ImplementationType), serviceProvider),
                    lifetime: ServiceLifetime.Scoped
                ));
            }

            return serviceCollection;
        }
#endif
    }
}
