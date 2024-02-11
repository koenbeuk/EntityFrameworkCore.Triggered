using System;
using System.Linq;
using System.Reflection;
using EntityFrameworkCore.Triggered;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;
using EntityFrameworkCore.Triggered.Lifecycles;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        readonly static Type[] _wellKnownTriggerTypes = [
                typeof(IBeforeSaveTrigger<>),
                typeof(IBeforeSaveAsyncTrigger<>),
                typeof(IAfterSaveTrigger<>),
                typeof(IAfterSaveAsyncTrigger<>),
                typeof(IAfterSaveFailedTrigger<>),
                typeof(IAfterSaveFailedAsyncTrigger<>),
                typeof(IBeforeSaveStartingTrigger),
                typeof(IBeforeSaveStartingAsyncTrigger),
                typeof(IBeforeSaveCompletedTrigger),
                typeof(IBeforeSaveCompletedAsyncTrigger),
                typeof(IAfterSaveFailedStartingTrigger),
                typeof(IAfterSaveFailedStartingAsyncTrigger),
                typeof(IAfterSaveFailedCompletedTrigger),
                typeof(IAfterSaveFailedCompletedAsyncTrigger),
                typeof(IAfterSaveStartingTrigger),
                typeof(IAfterSaveStartingAsyncTrigger),
                typeof(IAfterSaveCompletedTrigger),
                typeof(IAfterSaveCompletedAsyncTrigger)
            ];

        static void RegisterTriggerTypes(Type triggerImplementationType, IServiceCollection services)
        {
            foreach (var customTriggerType in _wellKnownTriggerTypes)
            {
                var customTriggers = customTriggerType.IsGenericTypeDefinition
#pragma warning disable EF1001 // Internal EF Core API usage.
                    ? TypeHelpers.FindGenericInterfaces(triggerImplementationType, customTriggerType)
#pragma warning restore EF1001 // Internal EF Core API usage.
                    : triggerImplementationType.GetInterfaces().Where(x => x == customTriggerType);

                foreach (var customTrigger in customTriggers)
                {
                    services.Add(new ServiceDescriptor(customTrigger, sp => sp.GetRequiredService(triggerImplementationType), ServiceLifetime.Transient)); ;
                }
            }
        }

        public static IServiceCollection AddTrigger<TTrigger>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TTrigger : class
        {
            services.TryAdd(new ServiceDescriptor(typeof(TTrigger), typeof(TTrigger), lifetime));

            RegisterTriggerTypes(typeof(TTrigger), services);

            return services;
        }

        public static IServiceCollection AddTrigger(this IServiceCollection services, object triggerInstance)
        {
            services.TryAddSingleton(triggerInstance);

            RegisterTriggerTypes(triggerInstance.GetType(), services);

            return services;
        }

        public static IServiceCollection AddAssemblyTriggers(this IServiceCollection services)
            => AddAssemblyTriggers(services, Assembly.GetCallingAssembly());

        public static IServiceCollection AddAssemblyTriggers(this IServiceCollection services, ServiceLifetime lifetime)
            => AddAssemblyTriggers(services, lifetime, Assembly.GetCallingAssembly());

        public static IServiceCollection AddAssemblyTriggers(this IServiceCollection services, params Assembly[] assemblies)
            => AddAssemblyTriggers(services, ServiceLifetime.Scoped, assemblies);

        public static IServiceCollection AddAssemblyTriggers(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
        {
            ArgumentNullException.ThrowIfNull(assemblies);

            var assemblyTypes = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .Where(x => !x.IsAbstract);

            foreach (var assemblyType in assemblyTypes)
            {
                var triggerTypes = assemblyType
                    .GetInterfaces()
                    .Where(x => _wellKnownTriggerTypes.Contains(x.IsConstructedGenericType ? x.GetGenericTypeDefinition() : x));

                var registered = false;

                foreach (var triggerType in triggerTypes)
                {
                    if (!registered)
                    {
                        services.TryAdd(new ServiceDescriptor(assemblyType, assemblyType, lifetime));

                        registered = true;
                    }

                    services.Add(new ServiceDescriptor(triggerType, sp => sp.GetRequiredService(assemblyType), ServiceLifetime.Transient));
                }
            }

            return services;
        }
    }
}
