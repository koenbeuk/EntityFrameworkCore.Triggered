using System;
using System.Linq;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
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
            serviceCollection.AddDbContext<TContext>(options => {
                optionsAction?.Invoke(options);
                options.UseTriggers();
            }, contextLifetime, optionsLifetime);

            return serviceCollection;
        }

        public static IServiceCollection AddTriggeredDbContextPool<TContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, int poolSize = 1024)
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
                    lifetime: ServiceLifetime.Transient
                ));
            }

            return serviceCollection;
        }

        public static IServiceCollection AddTriggeredDbContextPool<TContext, TImplementation>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, int poolSize = 1024)
            where TContext : class where TImplementation : DbContext, TContext
        {
            serviceCollection.AddDbContextPool<TContext, TImplementation>(options => {
                optionsAction?.Invoke(options);
                options.UseTriggers();
            }, poolSize);

            var serviceDescriptor = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(TContext));
            if (serviceDescriptor?.ImplementationFactory != null)
            {
                serviceCollection.Replace(ServiceDescriptor.Describe(
                    serviceType: typeof(TContext),
                    implementationFactory: serviceProvider => SetApplicationTriggerServiceProviderAccessor(serviceDescriptor.ImplementationFactory(serviceProvider), serviceProvider),
                    lifetime: ServiceLifetime.Transient
                ));
            }

            return serviceCollection;
        }

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
                    implementationFactory: serviceProvider => ActivatorUtilities.CreateInstance(serviceProvider, triggeredFactoryType, serviceProvider.GetRequiredService(serviceDescriptor.ImplementationType), serviceProvider),
                    lifetime: ServiceLifetime.Transient
                ));
            }

            return serviceCollection;
        }

        public static IServiceCollection AddTriggeredDbContextFactory<TContext, TFactory>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            where TContext : DbContext
            where TFactory : IDbContextFactory<TContext>
        {
            serviceCollection.AddDbContextFactory<TContext, TFactory>(options => {
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
                    implementationFactory: serviceProvider => ActivatorUtilities.CreateInstance(serviceProvider, triggeredFactoryType, serviceProvider.GetRequiredService(serviceDescriptor.ImplementationType), serviceProvider),
                    lifetime: ServiceLifetime.Transient
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
                    implementationFactory: serviceProvider => ActivatorUtilities.CreateInstance(serviceProvider, triggeredFactoryType, serviceProvider.GetRequiredService(serviceDescriptor.ImplementationType), serviceProvider),
                    lifetime: ServiceLifetime.Scoped
                ));
            }
            
            if (serviceDescriptor?.ImplementationFactory != null)
            {
                var triggeredFactoryType = typeof(TriggeredDbContextFactory<>).MakeGenericType(typeof(TContext));

                serviceCollection.Replace(ServiceDescriptor.Describe(
                    serviceType: typeof(IDbContextFactory<TContext>),
                    implementationFactory: serviceProvider => ActivatorUtilities.CreateInstance(serviceProvider, triggeredFactoryType, serviceDescriptor.ImplementationFactory),
                    lifetime: ServiceLifetime.Scoped
                ));
            }
            return serviceCollection;
        }
    }
}
