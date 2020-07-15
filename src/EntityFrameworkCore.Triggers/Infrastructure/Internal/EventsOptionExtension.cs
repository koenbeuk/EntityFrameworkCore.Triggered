using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Internal;
using EntityFrameworkCore.Triggers.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EntityFrameworkCore.Triggers.Infrastructure.Internal
{
    public class EventsOptionExtension : IDbContextOptionsExtension
    {
        sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private int? _serviceProviderHash;
            private string? _logFragment;
            public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
            {
            }

            public override bool IsDatabaseProvider => false;
            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        _logFragment = string.Empty;
                    }

                    return _logFragment;
                }
            }

            public override long GetServiceProviderHashCode()
            {
                if (_serviceProviderHash == null)
                {
                    var hashCode = nameof(EventsOptionExtension).GetHashCode();

                    var extension = (EventsOptionExtension)Extension;

                    if (extension.changeEventHandlers != null)
                    {
                        foreach (var eventHandler in extension.changeEventHandlers)
                        {
                            hashCode ^= eventHandler.GetHashCode();
                        }
                    }

                    hashCode ^= extension._maxRecursion.GetHashCode();
                    hashCode ^= extension._recursionMode.GetHashCode();

                    _serviceProviderHash = hashCode;
                }

                return _serviceProviderHash.Value;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                if (debugInfo == null)
                {
                    throw new ArgumentNullException(nameof(debugInfo));
                }

                debugInfo["ChangeEvents:HandlersCount"] = ((EventsOptionExtension)Extension).changeEventHandlers.Count().ToString();
                debugInfo["ChangeEvents:RecursionMode"] = ((EventsOptionExtension)Extension)._recursionMode.ToString();
                debugInfo["ChangeEvents:MaxRecursion"] = ((EventsOptionExtension)Extension)._maxRecursion.ToString();
            }
        }

        private ExtensionInfo? _info;
        private IEnumerable<(object typeOrInstance, ServiceLifetime lifetime)>? changeEventHandlers;
        private int _maxRecursion = 100;
        private RecursionMode _recursionMode = RecursionMode.EntityAndType;

        public EventsOptionExtension()
        {

        }

        public EventsOptionExtension(EventsOptionExtension copyFrom)
        {
            if (copyFrom.changeEventHandlers != null)
            {
                changeEventHandlers = copyFrom.changeEventHandlers;
            }
        }

        public DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        public int MaxRecursion => _maxRecursion;
        public RecursionMode RecursionMode => _recursionMode;
        public IEnumerable<(object typeOrInstance, ServiceLifetime lifetime)> ChangeEventHandlers => changeEventHandlers ?? Enumerable.Empty<(object typeOrInstance, ServiceLifetime lifetime)>();

        public void ApplyServices(IServiceCollection services)
        {
            services.AddLogging();
            
            services.TryAddScoped<IChangeEventService, ChangeEventService>();
            services.TryAddScoped<IChangeEventHandlerRegistryService, ChangeEventHandlerRegistryService>();

            var recursionStrategyType = _recursionMode switch
            {
                RecursionMode.None => typeof(NoRecursionStrategy),
                RecursionMode.EntityAndType => typeof(EntityAndTypeRecursionStrategy),
                _ => throw new InvalidOperationException("Unsupported recursion mode")
            };

            services.TryAddSingleton(typeof(IRecursionStrategy), recursionStrategyType);

            if (changeEventHandlers != null)
            {
                foreach (var (typeOrInstance, lifetime) in changeEventHandlers)
                {
                    var (eventHandlerType, eventHandlerInstance) = typeOrInstance switch
                    {
                        Type type => (type, null),
                        object instance => (instance.GetType(), instance),
                        _ => throw new InvalidOperationException("Unknown type registration")
                    };

                    var beforeSaveChangeEventHandlerType = TypeHelpers.FindGenericInterface(eventHandlerType, typeof(IBeforeSaveChangeEventHandler<>));
                    var afterSaveChangeEventHandler = TypeHelpers.FindGenericInterface(eventHandlerType, typeof(IAfterSaveChangeEventHandler<>));

                    if (beforeSaveChangeEventHandlerType != null)
                    {
                        if (eventHandlerInstance != null)
                        {
                            services.Add(new ServiceDescriptor(beforeSaveChangeEventHandlerType, eventHandlerInstance));
                        }
                        else
                        {
                            services.Add(new ServiceDescriptor(beforeSaveChangeEventHandlerType, eventHandlerType, lifetime));
                        }
                    }

                    if (afterSaveChangeEventHandler != null)
                    {
                        if (eventHandlerInstance != null)
                        {
                            services.Add(new ServiceDescriptor(afterSaveChangeEventHandler, eventHandlerInstance));
                        }
                        else
                        {
                            services.Add(new ServiceDescriptor(afterSaveChangeEventHandler, eventHandlerType, lifetime));
                        }
                    }
                }
            }
        }

        public void Validate(IDbContextOptions options) { }

        protected EventsOptionExtension Clone() => new EventsOptionExtension(this);

        private static bool TypeIsValidEventHandler(Type type) 
            => TypeHelpers.FindGenericInterface(type, typeof(IBeforeSaveChangeEventHandler<>)) != null || TypeHelpers.FindGenericInterface(type, typeof(IAfterSaveChangeEventHandler<>)) != null;
        public EventsOptionExtension WithRecursionMode(RecursionMode recursionMode)
        {
            var clone = Clone();

            clone._recursionMode = recursionMode;

            return clone;
        }

        public EventsOptionExtension WithMaxRecursion(int maxRecursion)
        {
            var clone = Clone();

            clone._maxRecursion = maxRecursion;

            return clone;
        }

        public EventsOptionExtension WithAdditionalChangeEventHandler(Type eventHandlerType, ServiceLifetime lifetime)
        {
            if (!TypeIsValidEventHandler(eventHandlerType))
            {
                throw new ArgumentException("An event handler needs to implement either or both IBeforeSaveChangeEventHandler or IAfterSaveChangeEventHandler", nameof(eventHandlerType));
            }

            var clone = Clone();
            var eventHandlerEnumerable = Enumerable.Repeat(((object)eventHandlerType, lifetime), 1);

            if (clone.changeEventHandlers == null)
            {
                clone.changeEventHandlers = eventHandlerEnumerable;
            }
            else
            {
                clone.changeEventHandlers = clone.changeEventHandlers.Concat(eventHandlerEnumerable);
            }
            

            return clone;
        }

        public EventsOptionExtension WithAdditionalEventHandler(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (!TypeIsValidEventHandler(instance.GetType()))
            {
                throw new ArgumentException("An event handler needs to implement either or both IBeforeSaveChangeEventHandler or IAfterSaveChangeEventHandler", nameof(instance));
            }

            var clone = Clone();
            var eventHandlerEnumerable = Enumerable.Repeat((instance, ServiceLifetime.Singleton), 1);

            if (clone.changeEventHandlers == null)
            {
                clone.changeEventHandlers = eventHandlerEnumerable;
            }
            else
            {
                clone.changeEventHandlers = clone.changeEventHandlers.Concat(eventHandlerEnumerable);
            }


            return clone;
        }
    }
}
