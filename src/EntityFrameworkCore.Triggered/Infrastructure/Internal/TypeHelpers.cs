using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EntityFrameworkCore.Triggered.Infrastructure.Internal
{
    public static class TypeHelpers
    {
        public static IEnumerable<Type> FindGenericInterfaces(Type type, Type interfaceType)
            => type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);

        public static IEnumerable<Type> EnumerateTypeHierarchy(Type type)
        {
            Type? nextType = type;

            while(nextType is not null)
            {
                yield return nextType;
                nextType = nextType.BaseType;
            } while (nextType != null);
        }
    }
}
