using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Infrastructure
{
    public class TriggersContextOptionsBuilder
    {
        readonly DbContextOptionsBuilder _optionsBuilder;

        public TriggersContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        {
            _optionsBuilder = optionsBuilder ?? throw new ArgumentNullException(nameof(optionsBuilder));
        }

        public TriggersContextOptionsBuilder AddTrigger<TTrigger>()
            => AddTrigger<TTrigger>(ServiceLifetime.Transient);

        public TriggersContextOptionsBuilder AddTrigger<TTrigger>(ServiceLifetime lifetime)
            => AddTrigger(typeof(TTrigger), lifetime);

        public TriggersContextOptionsBuilder AddTrigger(Type triggerType, ServiceLifetime lifetime)
            => WithOption(e => e.WithAdditionalTrigger(triggerType, lifetime));

        public TriggersContextOptionsBuilder AddTrigger(object trigger)
            => WithOption(e => e.WithAdditionalTrigger(trigger));

        public TriggersContextOptionsBuilder RecursionMode(RecursionMode recursionMode = Infrastructure.RecursionMode.EntityAndType)
            => WithOption(e => e.WithRecursionMode(recursionMode));

        public TriggersContextOptionsBuilder MaxRecusion(int maxRecursion = 100)
            => WithOption(e => e.WithMaxRecursion(maxRecursion));

        public TriggersContextOptionsBuilder AddTriggerType(Type triggerType)
            => WithOption(e => e.WithAdditionalTriggerType(triggerType));

        public TriggersContextOptionsBuilder UseApplicationScopedServiceProviderAccessor(Func<IServiceProvider, IServiceProvider> serviceProviderTransform)
            => WithOption(e => e.WithApplicationScopedServiceProviderAccessor(serviceProviderTransform));

        /// <summary>
        ///     Sets an option by cloning the extension used to store the settings. This ensures the builder
        ///     does not modify options that are already in use elsewhere.
        /// </summary>
        /// <param name="setAction"> An action to set the option. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        protected virtual TriggersContextOptionsBuilder WithOption(Func<TriggersOptionExtension, TriggersOptionExtension> setAction)
        {
            ((IDbContextOptionsBuilderInfrastructure)_optionsBuilder).AddOrUpdateExtension(
                setAction(_optionsBuilder.Options.FindExtension<TriggersOptionExtension>() ?? new TriggersOptionExtension()));

            return this;
        }
    }
}
