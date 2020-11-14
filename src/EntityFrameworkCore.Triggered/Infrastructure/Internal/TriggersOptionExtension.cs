using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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

                    if (extension._triggerTypes != null)
                    {
                        foreach (var triggerType in extension._triggerTypes)
                        {
                            hashCode ^= triggerType.GetHashCode();
                        }
                    }

                    hashCode ^= extension._maxRecursion.GetHashCode();
                    hashCode ^= extension._recursionMode.GetHashCode();

                    if (extension._serviceProviderTransform != null)
                    {
                        hashCode ^= extension._serviceProviderTransform.GetHashCode();
                    }

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

                debugInfo["Triggers:TriggersCount"] = (((TriggersOptionExtension)Extension)._triggers?.Count() ?? 0).ToString();
                debugInfo["Triggers:AdditionalTriggerTypesCount"] = (((TriggersOptionExtension)Extension)._triggerTypes?.Count() ?? 0).ToString();
                debugInfo["Triggers:MaxRecursion"] = ((TriggersOptionExtension)Extension)._maxRecursion.ToString();
                debugInfo["Triggers:RecursionMode"] = ((TriggersOptionExtension)Extension)._recursionMode.ToString();
            }
        }

        private ExtensionInfo? _info;
        private IEnumerable<(object typeOrInstance, ServiceLifetime lifetime)>? _triggers;
        private IEnumerable<Type>? _triggerTypes;
        private int _maxRecursion = 100;
        private RecursionMode _recursionMode = RecursionMode.EntityAndType;
        private Func<IServiceProvider, IServiceProvider>? _serviceProviderTransform;

        public TriggersOptionExtension()
        {

        }

        public TriggersOptionExtension(TriggersOptionExtension copyFrom)
        {
            if (copyFrom._triggers != null)
            {
                _triggers = copyFrom._triggers;
            }

            if (copyFrom._triggerTypes != null)
            {
                _triggerTypes = copyFrom._triggerTypes;
            }

            _maxRecursion = copyFrom._maxRecursion;
            _recursionMode = copyFrom._recursionMode;
            _serviceProviderTransform = copyFrom._serviceProviderTransform;
        }

        public DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        public int MaxRecursion => _maxRecursion;
        public RecursionMode RecursionMode => _recursionMode;
        public IEnumerable<(object typeOrInstance, ServiceLifetime lifetime)> Triggers => _triggers ?? Enumerable.Empty<(object typeOrInstance, ServiceLifetime lifetime)>();

        public void ApplyServices(IServiceCollection services)
        {
            services.AddScoped(serviceProvider => new ApplicationTriggerServiceProviderAccessor(serviceProvider, _serviceProviderTransform, serviceProvider.GetService<ILogger<ApplicationTriggerServiceProviderAccessor>>()));
            services.AddScoped<IResettableService>(serviceProvider => serviceProvider.GetRequiredService<ApplicationTriggerServiceProviderAccessor>());
            services.TryAddScoped<ITriggerServiceProviderAccessor>(serviceProvider => serviceProvider.GetRequiredService<ApplicationTriggerServiceProviderAccessor>());

            services.TryAddSingleton<ITriggerTypeRegistryService, TriggerTypeRegistryService>();
            services.TryAddScoped<ITriggerDiscoveryService, TriggerDiscoveryService>();

            services.TryAddScoped<TriggerService>();
            services.TryAddScoped<IResettableService>(serviceProvider => serviceProvider.GetRequiredService<TriggerService>());
            services.TryAddScoped<ITriggerService>(serviceProvider => serviceProvider.GetRequiredService<TriggerService>());

#if EFCORETRIGGERED2
            services.TryAddScoped<IInterceptor, TriggerSessionSaveChangesInterceptor>();
#endif


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
                    var afterSaveFailedTriggers = TypeHelpers.FindGenericInterfaces(triggerType, typeof(IAfterSaveFailedTrigger<>));

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

                    foreach (var afterSaveFailedTrigger in afterSaveFailedTriggers)
                    {
                        if (triggerInstance != null)
                        {
                            services.Add(new ServiceDescriptor(afterSaveFailedTrigger, triggerInstance));
                        }
                        else
                        {
                            services.Add(new ServiceDescriptor(afterSaveFailedTrigger, triggerType, lifetime));
                        }
                    }

                    if (_triggerTypes != null)
                    {
                        foreach (var customTriggerType in _triggerTypes.Distinct())
                        {
                            var customTriggers = TypeHelpers.FindGenericInterfaces(triggerType, customTriggerType);

                            foreach (var customTrigger in customTriggers)
                            {
                                if (triggerInstance != null)
                                {
                                    services.Add(new ServiceDescriptor(customTrigger, triggerInstance));
                                }
                                else
                                {
                                    services.Add(new ServiceDescriptor(customTrigger, triggerType, lifetime));
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Validate(IDbContextOptions options) { }

        protected TriggersOptionExtension Clone() => new(this);

        private bool TypeIsValidTrigger(Type type)
        {
            if (TypeHelpers.FindGenericInterfaces(type, typeof(IBeforeSaveTrigger<>)) != null || TypeHelpers.FindGenericInterfaces(type, typeof(IAfterSaveTrigger<>)) != null)
            {
                return true;
            }
            else if (_triggerTypes != null)
            {
                return _triggerTypes.Any(triggerType => TypeHelpers.FindGenericInterfaces(type, triggerType) != null);
            }
            else
            {
                return false;
            }
        }

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

        public TriggersOptionExtension WithAdditionalTriggerType(Type triggerType)
        {
            if (triggerType == null)
            {
                throw new ArgumentNullException(nameof(triggerType));
            }


            var clone = Clone();
            var triggerTypesEnumerable = Enumerable.Repeat(triggerType, 1);

            if (clone._triggerTypes == null)
            {
                clone._triggerTypes = triggerTypesEnumerable;
            }
            else
            {
                clone._triggerTypes = clone._triggerTypes.Concat(triggerTypesEnumerable);
            }


            return clone;
        }

        public TriggersOptionExtension WithApplicationScopedServiceProviderAccessor(Func<IServiceProvider, IServiceProvider> serviceProviderTransform)
        {
            var clone = Clone();
            clone._serviceProviderTransform = serviceProviderTransform;

            return clone;
        }
    }
}
