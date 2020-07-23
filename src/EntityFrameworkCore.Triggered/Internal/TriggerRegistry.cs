using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerRegistry
    {
        readonly static ConcurrentDictionary<Type, List<Type>> _cachedTriggerTypes = new ConcurrentDictionary<Type, List<Type>>();

        readonly Type _changeHandlerType;
        readonly IServiceProvider _applicationServiceProvider;
        readonly Func<object, TriggerAdapterBase> _executionStrategyFactory;

        public TriggerRegistry(Type changeHandlerType, IServiceProvider applicationServiceProvider, Func<object, TriggerAdapterBase> executionStrategyFactory)
        {
            if (!changeHandlerType.IsGenericTypeDefinition || changeHandlerType.GenericTypeArguments.Length == 1)
            {
                // todo: add detail
                throw new ArgumentException("A valid change handler type should accept 1 type argument and contain just 1 method", nameof(changeHandlerType));
            }

            _applicationServiceProvider = applicationServiceProvider ?? throw new ArgumentNullException(nameof(applicationServiceProvider));
            _changeHandlerType = changeHandlerType;
            _executionStrategyFactory = executionStrategyFactory;
        }

        private IReadOnlyCollection<Type> GetTriggerTypes(Type entityType)
        {
            return _cachedTriggerTypes.GetOrAdd(entityType, entityType => {
                var result = new List<Type>();

                // Enumerable of the type hierarchy from base to concrete
                var typeHierarchy = TypeHelpers.EnumerateTypeHierarchy(entityType).Reverse();
                foreach (var type in typeHierarchy)
                {
                    foreach (var interfaceType in type.GetInterfaces())
                    {
                        result.Add((_changeHandlerType).MakeGenericType(interfaceType));
                    }

                    result.Add((_changeHandlerType).MakeGenericType(type));
                }

                return result;
            });
        }

        public IEnumerable<TriggerAdapterBase> DiscoverTriggers(Type entityType)
        {
            return GetTriggerTypes(entityType)
                .SelectMany(triggerType => _applicationServiceProvider.GetServices(triggerType))
                .Distinct()
                .Select(trigger => _executionStrategyFactory(trigger))
                .OrderBy(x => x.Priority);
        }
    }
}
