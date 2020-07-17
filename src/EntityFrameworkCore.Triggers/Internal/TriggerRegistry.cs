using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggers.Internal
{
    public sealed class TriggerRegistry
    {
        readonly Type _changeHandlerType;
        readonly IServiceProvider _serviceProvider;
        readonly Func<object, TriggerAdapterBase> _executionStrategyFactory;

        public TriggerRegistry(Type changeHandlerType, IServiceProvider serviceProvider, Func<object, TriggerAdapterBase> executionStrategyFactory)
        {
            if (!changeHandlerType.IsGenericTypeDefinition || changeHandlerType.GenericTypeArguments.Length == 1)
            {
                // todo: add detail
                throw new ArgumentException("A valid change handler type should accept 1 type argument and contain just 1 method", nameof(changeHandlerType));
            }

            _changeHandlerType = changeHandlerType;
            _serviceProvider = serviceProvider;
            _executionStrategyFactory = executionStrategyFactory;
        }

        public IEnumerable<TriggerAdapterBase> DiscoverChangeHandlers(Type entityType)
        {
            return entityType
                .GetInterfaces()
                .Concat(TypeHelpers.EnumerateTypeHierarchy(entityType))
                .Select(type => (_changeHandlerType).MakeGenericType(type))
                .SelectMany(type => _serviceProvider.GetServices(type))
                .Select(changeHandler => _executionStrategyFactory(changeHandler));
        }
    }
}
