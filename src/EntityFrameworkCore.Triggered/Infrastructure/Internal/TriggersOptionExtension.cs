using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EntityFrameworkCore.Triggered.Infrastructure.Internal
{
    public class TriggersOptionExtension : IDbContextOptionsExtension
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
                    var hashCode = nameof(TriggersOptionExtension).GetHashCode();

                    var extension = (TriggersOptionExtension)Extension;

                    if (extension._triggers != null)
                    {
                        foreach (var trigger in extension._triggers)
                        {
                            hashCode ^= trigger.GetHashCode();
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

                debugInfo["Triggers:HandlersCount"] = (((TriggersOptionExtension)Extension)._triggers?.Count() ?? 0).ToString();
                debugInfo["Triggers:MaxRecursion"] = ((TriggersOptionExtension)Extension)._maxRecursion.ToString();
                debugInfo["Triggers:RecursionMode"] = ((TriggersOptionExtension)Extension)._recursionMode.ToString();
            }
        }

        private ExtensionInfo? _info;
        private IEnumerable<(object typeOrInstance, ServiceLifetime lifetime)>? _triggers;
        private int _maxRecursion = 100;
        private RecursionMode _recursionMode = RecursionMode.EntityAndType;

        public TriggersOptionExtension()
        {

        }

        public TriggersOptionExtension(TriggersOptionExtension copyFrom)
        {
            if (copyFrom._triggers != null)
            {
                _triggers = copyFrom._triggers;
            }

            _maxRecursion = copyFrom._maxRecursion;
            _recursionMode = copyFrom._recursionMode;
        }

        public DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        public int MaxRecursion => _maxRecursion;
        public RecursionMode RecursionMode => _recursionMode;
        public IEnumerable<(object typeOrInstance, ServiceLifetime lifetime)> Triggers => _triggers ?? Enumerable.Empty<(object typeOrInstance, ServiceLifetime lifetime)>();

        public void ApplyServices(IServiceCollection services)
        {
            services.TryAddScoped<ITriggerRegistryService>(serviceProvider => {
                // todo: Find a better way of getting our hands on the ApplicationServiceProvider
                var dbContextOptions = serviceProvider.GetRequiredService<IDbContextOptions>();
                var coreOptionsExtension = dbContextOptions.FindExtension<CoreOptionsExtension>();
                
                return new TriggerRegistryService(serviceProvider, coreOptionsExtension.ApplicationServiceProvider);
            });
            services.TryAddScoped<ITriggerService, TriggerService>();

            services.Configure<TriggerOptions>(triggerServiceOptions => {
                triggerServiceOptions.MaxRecursion = _maxRecursion;
            });

            var recursionStrategyType = _recursionMode switch
            {
                RecursionMode.None => typeof(NoRecursionStrategy),
                RecursionMode.EntityAndType => typeof(EntityAndTypeRecursionStrategy),
                _ => throw new InvalidOperationException("Unsupported recursion mode")
            };

            services.TryAddTransient(typeof(IRecursionStrategy), recursionStrategyType);

            if (_triggers != null)
            {
                foreach (var (typeOrInstance, lifetime) in _triggers)
                {
                    var (triggerType, triggerInstance) = typeOrInstance switch
                    {
                        Type type => (type, null),
                        object instance => (instance.GetType(), instance),
                        _ => throw new InvalidOperationException("Unknown type registration")
                    };

                    var beforeSaveChangeTriggers = TypeHelpers.FindGenericInterfaces(triggerType, typeof(IBeforeSaveTrigger<>));
                    var afterSaveChangeTriggers = TypeHelpers.FindGenericInterfaces(triggerType, typeof(IAfterSaveTrigger<>));

                    foreach (var beforeSaveChangeTrigger in beforeSaveChangeTriggers)
                    {
                        if (triggerInstance != null)
                        {
                            services.Add(new ServiceDescriptor(beforeSaveChangeTrigger, triggerInstance));
                        }
                        else
                        {
                            services.Add(new ServiceDescriptor(beforeSaveChangeTrigger, triggerType, lifetime));
                        }
                    }

                    foreach (var afterSaveChangeTrigger in afterSaveChangeTriggers)
                    {
                        if (triggerInstance != null)
                        {
                            services.Add(new ServiceDescriptor(afterSaveChangeTrigger, triggerInstance));
                        }
                        else
                        {
                            services.Add(new ServiceDescriptor(afterSaveChangeTrigger, triggerType, lifetime));
                        }
                    }
                }
            }
        }

        public void Validate(IDbContextOptions options) { }

        protected TriggersOptionExtension Clone() => new TriggersOptionExtension(this);

        private static bool TypeIsValidTrigger(Type type) 
            => TypeHelpers.FindGenericInterfaces(type, typeof(IBeforeSaveTrigger<>)) != null || TypeHelpers.FindGenericInterfaces(type, typeof(IAfterSaveTrigger<>)) != null;
        public TriggersOptionExtension WithRecursionMode(RecursionMode recursionMode)
        {
            var clone = Clone();

            clone._recursionMode = recursionMode;

            return clone;
        }

        public TriggersOptionExtension WithMaxRecursion(int maxRecursion)
        {
            var clone = Clone();

            clone._maxRecursion = maxRecursion;

            return clone;
        }

        public TriggersOptionExtension WithAdditionalTrigger(Type triggerType, ServiceLifetime lifetime)
        {
            if (!TypeIsValidTrigger(triggerType))
            {
                throw new ArgumentException("A trigger needs to implement either or both IBeforeSaveChangeTrigger or IAfterSaveChangeTriggerHandler", nameof(triggerType));
            }

            var clone = Clone();
            var triggerEnumerable = Enumerable.Repeat(((object)triggerType, lifetime), 1);

            if (clone._triggers == null)
            {
                clone._triggers = triggerEnumerable;
            }
            else
            {
                clone._triggers = clone._triggers.Concat(triggerEnumerable);
            }
            

            return clone;
        }

        public TriggersOptionExtension WithAdditionalTrigger(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (!TypeIsValidTrigger(instance.GetType()))
            {
                throw new ArgumentException("A trigger needs to implement either or both IBeforeSaveChangeTrigger or IAfterSaveChangeTriggerHandler", nameof(instance));
            }

            var clone = Clone();
            var triggersEnumerable = Enumerable.Repeat((instance, ServiceLifetime.Singleton), 1);

            if (clone._triggers == null)
            {
                clone._triggers = triggersEnumerable;
            }
            else
            {
                clone._triggers = clone._triggers.Concat(triggersEnumerable);
            }


            return clone;
        }
    }
}
