using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggers.Infrastructure
{
    public class EventsContextOptionsBuilder
    {
        readonly DbContextOptionsBuilder _optionsBuilder;

        public EventsContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
        {
            _optionsBuilder = optionsBuilder ?? throw new ArgumentNullException(nameof(optionsBuilder));
        }

        public EventsContextOptionsBuilder AddChangeEventHandler<TEventHandler>()
            => AddChangeEventHandler<TEventHandler>(ServiceLifetime.Transient);

        public EventsContextOptionsBuilder AddChangeEventHandler<TEventHandler>(ServiceLifetime lifetime)
            => AddChangeEventHandler(typeof(TEventHandler), lifetime);

        public EventsContextOptionsBuilder AddChangeEventHandler(Type eventHandlerType, ServiceLifetime lifetime)
            => WithOption(e => e.WithAdditionalChangeEventHandler(eventHandlerType, lifetime));

        public EventsContextOptionsBuilder AddChangeEventHandler(object eventHandler)
            => WithOption(e => e.WithAdditionalEventHandler(eventHandler));

        public EventsContextOptionsBuilder RecursionMode(RecursionMode recursionMode = Infrastructure.RecursionMode.EntityAndType)
            => WithOption(e => e.WithRecursionMode(recursionMode));

        public EventsContextOptionsBuilder MaxRecusion(int maxRecursion = 100)
            => WithOption(e => e.WithMaxRecursion(maxRecursion));

        /// <summary>
        ///     Sets an option by cloning the extension used to store the settings. This ensures the builder
        ///     does not modify options that are already in use elsewhere.
        /// </summary>
        /// <param name="setAction"> An action to set the option. </param>
        /// <returns> The same builder instance so that multiple calls can be chained. </returns>
        protected virtual EventsContextOptionsBuilder WithOption(Func<EventsOptionExtension, EventsOptionExtension> setAction)
        {
            ((IDbContextOptionsBuilderInfrastructure)_optionsBuilder).AddOrUpdateExtension(
                setAction(_optionsBuilder.Options.FindExtension<EventsOptionExtension>() ?? new EventsOptionExtension()));

            return this;
        }
    }
}
