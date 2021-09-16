using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal
{
    public static class TriggerContextFactory<TEntityType>
        where TEntityType : class
    {
        readonly static Func<object, PropertyValues?, ChangeType, EntityBagStateManager, TriggerContext<TEntityType>> _factoryMethod = CreateFactoryMethod();

        static Func<object, PropertyValues?, ChangeType, EntityBagStateManager, TriggerContext<TEntityType>> CreateFactoryMethod()
        {
            var entityParamExpression = Expression.Parameter(typeof(object), "object");
            var originalValuesParamExpression = Expression.Parameter(typeof(PropertyValues), "originalValues");
            var changeTypeParamExpression = Expression.Parameter(typeof(ChangeType), "changeType");
            var entityBagStateManagerExpression = Expression.Parameter(typeof(EntityBagStateManager), "entityBagStateManager");

            return Expression.Lambda<Func<object, PropertyValues?, ChangeType, EntityBagStateManager, TriggerContext<TEntityType>>>(
                Expression.New(
                    typeof(TriggerContext<>).MakeGenericType(typeof(TEntityType)).GetConstructor(new[] { typeof(object), typeof(PropertyValues), typeof(ChangeType), typeof(EntityBagStateManager) })!,
                    entityParamExpression,
                    originalValuesParamExpression,
                    changeTypeParamExpression,
                    entityBagStateManagerExpression
                ),
                entityParamExpression,
                originalValuesParamExpression,
                changeTypeParamExpression,
                entityBagStateManagerExpression
            )
            .Compile();
        }

        public static object Activate(object entity, PropertyValues? originalValues, ChangeType changeType, EntityBagStateManager entityBagStateManager)
            => _factoryMethod(entity, originalValues, changeType, entityBagStateManager);
    }
}
