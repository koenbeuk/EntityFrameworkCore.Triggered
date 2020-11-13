using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
#if EFCORETRIGGERED2
        [Obsolete("AddAspNetCoreTriggeredDbContext is obsolete and can be replaced by a call to AddTriggeredDbContext instead. EntityFrameworkCore.Triggered.AspNetCore is no longer needed")]
#endif
        public static IServiceCollection AddAspNetCoreTriggeredDbContext<TContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder>? optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped) where TContext : DbContext
        {
            if (serviceCollection is null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            serviceCollection.AddTriggeredDbContext<TContext>(options => {
                optionsAction?.Invoke(options);

                options.UseTriggers(triggerOptions => {
                    triggerOptions.UseAspNetCoreIntegration();
                });

            }, contextLifetime, optionsLifetime);

            return serviceCollection;
        }
    }
}
