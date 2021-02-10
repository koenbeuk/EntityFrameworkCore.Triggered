using System;
using System.Linq;
using System.Reflection;
using EntityFrameworkCore.Triggered;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;
using EntityFrameworkCore.Triggered.Lyfecycles;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        static readonly Type[] _triggerTypes = new Type[] {
                typeof(IBeforeSaveTrigger<>),
                typeof(IAfterSaveTrigger<>),
                typeof(IAfterSaveFailedTrigger<>),
                typeof(IBeforeSaveStartingTrigger),
                typeof(IBeforeSaveCompletedTrigger),
                typeof(IAfterSaveFailedStartingTrigger),
                typeof(IAfterSaveFailedCompletedTrigger),
                typeof(IAfterSaveStartingTrigger),
                typeof(IAfterSaveCompletedTrigger)
            };

        static void RegisterTriggerTypes(Type triggerImplementationType, IServiceCollection services)
        {
            foreach (var customTriggerType in _triggerTypes)
            {
                var customTriggers = customTriggerType.IsGenericTypeDefinition
#pragma warning disable EF1001 // Internal EF Core API usage.
                    ? TypeHelpers.FindGenericInterfaces(triggerImplementationType, customTriggerType)
#pragma warning restore EF1001 // Internal EF Core API usage.
                    : triggerImplementationType.GetInterfaces().Where(x => x == customTriggerType);

                foreach (var customTrigger in customTriggers)
                {
                    services.TryAdd(new ServiceDescriptor(customTrigger, triggerImplementationType, ServiceLifetime.Transient)); ;
                }
            }
        }

        public static IServiceCollection AddTrigger<TTrigger>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TTrigger : class
        {
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient<TTrigger>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<TTrigger>();
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton<TTrigger>();
                    break;
                default:
                    throw new InvalidOperationException("Unknown lifetime");
            }

            RegisterTriggerTypes(typeof(TTrigger), services);

            return services;
        }

        public static IServiceCollection AddTrigger(this IServiceCollection services, object triggerInstance)
        {
            services.AddSingleton(triggerInstance);

            RegisterTriggerTypes(triggerInstance.GetType(), services);

            return services;
        }

        public static IServiceCollection AddAssemblyTriggers(this IServiceCollection services)
            => AddAssemblyTriggers(services, Assembly.GetCallingAssembly());

        public static IServiceCollection AddAssemblyTriggers(this IServiceCollection services, params Assembly[] assemblies)
            => AddAssemblyTriggers(services, ServiceLifetime.Scoped, assemblies);

        public static IServiceCollection AddAssemblyTriggers(this IServiceCollection services, ServiceLifetime lifetime, params Assembly[] assemblies)
        {
            if (assemblies is null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var assemblyTypes = assemblies.SelectMany(x => x.GetTypes());

            foreach (var assemblyType in assemblyTypes)
            {
                var triggerTypeCandidates = assemblyType
                    .GetInterfaces()
                    .SelectMany(x => x.GetInterfaces())
                    .Select(x => (genericTriggerType: x.IsGenericTypeDefinition ? x.GetGenericTypeDefinition() : x, triggerType: x));

                var triggerTypes = _triggerTypes
                    .Join(triggerTypeCandidates, x => x, x => x.genericTriggerType, (_, x) => x.triggerType);

                var registered = false;

                foreach (var triggerType in triggerTypes)
                {
                    if (!registered)
                    {
                        services.Add(new ServiceDescriptor(assemblyType, assemblyType, lifetime));

                        registered = true;
                    }

                    services.TryAdd(new ServiceDescriptor(triggerType, assemblyType, ServiceLifetime.Transient));
                }
            }

            return services;
        }
    }
}
