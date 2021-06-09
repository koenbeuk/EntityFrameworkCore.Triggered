using System;
using System.Linq;
using System.Reflection;
using EntityFrameworkCore.Triggered.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    public static class TriggersContextOptionsBuilderExtensions
    {
        public static TriggersContextOptionsBuilder AddAssemblyTriggers(this TriggersContextOptionsBuilder builder)
            => AddAssemblyTriggers(builder, Assembly.GetCallingAssembly());

        public static TriggersContextOptionsBuilder AddAssemblyTriggers(this TriggersContextOptionsBuilder builder, ServiceLifetime lifetime)
            => AddAssemblyTriggers(builder, lifetime, Assembly.GetCallingAssembly());

        public static TriggersContextOptionsBuilder AddAssemblyTriggers(this TriggersContextOptionsBuilder builder, params Assembly[] assemblies)
            => AddAssemblyTriggers(builder, ServiceLifetime.Scoped, assemblies);

        public static TriggersContextOptionsBuilder AddAssemblyTriggers(this TriggersContextOptionsBuilder builder, ServiceLifetime lifetime, params Assembly[] assemblies)
        {
            if (assemblies is null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            var assemblyTypes = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass);

            foreach (var assemblyType in assemblyTypes)
            {
                builder.AddTrigger(assemblyType, lifetime);
            }

            return builder;
        }

    }
}
