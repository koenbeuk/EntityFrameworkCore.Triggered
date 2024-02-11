using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal;

public static class TriggerContextFactory<TEntityType>
    where TEntityType : class
{
    readonly static Func<EntityEntry, PropertyValues?, ChangeType, EntityBagStateManager, TriggerContext<TEntityType>> _factoryMethod = CreateFactoryMethod();

    static Func<EntityEntry, PropertyValues?, ChangeType, EntityBagStateManager, TriggerContext<TEntityType>> CreateFactoryMethod()
    {
        var entityEntryParamExpression = Expression.Parameter(typeof(EntityEntry), "entityEntry");
        var originalValuesParamExpression = Expression.Parameter(typeof(PropertyValues), "originalValues");
        var changeTypeParamExpression = Expression.Parameter(typeof(ChangeType), "changeType");
        var entityBagStateManagerExpression = Expression.Parameter(typeof(EntityBagStateManager), "entityBagStateManager");

        return Expression.Lambda<Func<EntityEntry, PropertyValues?, ChangeType, EntityBagStateManager, TriggerContext<TEntityType>>>(
            Expression.New(
                typeof(TriggerContext<>).MakeGenericType(typeof(TEntityType)).GetConstructor([typeof(EntityEntry), typeof(PropertyValues), typeof(ChangeType), typeof(EntityBagStateManager)])!,
                entityEntryParamExpression,
                originalValuesParamExpression,
                changeTypeParamExpression,
                entityBagStateManagerExpression
            ),
            entityEntryParamExpression,
            originalValuesParamExpression,
            changeTypeParamExpression,
            entityBagStateManagerExpression
        )
        .Compile();
    }

    public static object Activate(EntityEntry entityEntry, PropertyValues? originalValues, ChangeType changeType, EntityBagStateManager entityBagStateManager)
        => _factoryMethod(entityEntry, originalValues, changeType, entityBagStateManager);
}
