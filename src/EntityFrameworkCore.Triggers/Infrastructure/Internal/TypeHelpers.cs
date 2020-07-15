using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityFrameworkCore.Triggers.Infrastructure.Internal
{
    public static class TypeHelpers
    {
        public static Type? FindGenericInterface(Type type, Type interfaceType) 
            => type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType).FirstOrDefault();

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
