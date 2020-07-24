using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal
{
    public static class TriggerContextFactory<TEntityType>
        where TEntityType: class
    {
        static readonly Func<EntityEntry, ChangeType, TriggerContext<TEntityType>> _factoryMethod = CreateFactoryMethod();

        static Func<EntityEntry, ChangeType, TriggerContext<TEntityType>> CreateFactoryMethod()
        {
            var entityTypeParamExpression = Expression.Parameter(typeof(EntityEntry), "entityEntry");
            var changeTypeParamExpression = Expression.Parameter(typeof(ChangeType), "changeType");

            return Expression.Lambda<Func<EntityEntry, ChangeType, TriggerContext<TEntityType>>>(
                Expression.New(
                    typeof(TriggerContext<>).MakeGenericType(typeof(TEntityType)).GetConstructor(new[] { typeof(EntityEntry), typeof(ChangeType) }),
                    entityTypeParamExpression,
                    changeTypeParamExpression
                ),
                entityTypeParamExpression,
                changeTypeParamExpression
            )
            .Compile();
        }

        public static object Activate(EntityEntry entry, ChangeType changeType)
            => _factoryMethod(entry, changeType);
    }
}
