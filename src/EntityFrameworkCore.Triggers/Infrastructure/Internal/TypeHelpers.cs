using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFrameworkCore.Triggers.Infrastructure.Internal
{
    public static class TypeHelpers
    {
        public static IEnumerable<Type> FindGenericInterfaces(Type type, Type interfaceType)
            => type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);

        public static IEnumerable<Type> EnumerateTypeHierarchy(Type type)
        {
            do
            {
                yield return type;
                type = type.BaseType;
            } while (type != null);
        }
    }
}
