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
    public sealed class ChangeEventHandlerRegistry
    {
        readonly Type _changeHandlerType;
        readonly IServiceProvider _serviceProvider;
        readonly Func<object, ChangeEventHandlerExecutionAdapterBase> _executionStrategyFactory;

        public ChangeEventHandlerRegistry(Type changeHandlerType, IServiceProvider serviceProvider, Func<object, ChangeEventHandlerExecutionAdapterBase> executionStrategyFactory)
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

        public IEnumerable<ChangeEventHandlerExecutionAdapterBase> DiscoverChangeHandlers(Type entityType)
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
