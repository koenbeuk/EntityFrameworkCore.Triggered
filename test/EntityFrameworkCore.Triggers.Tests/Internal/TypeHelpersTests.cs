using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Infrastructure.Internal;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests.Internal
{
    public class TypeHelpersTests
    {
        [Theory]
        [InlineData(typeof(ChangeEventHandler<>), typeof(IBeforeSaveChangeEventHandler<>), true)]
        [InlineData(typeof(ChangeEventHandler<object>), typeof(IBeforeSaveChangeEventHandler<>), true)]
        [InlineData(typeof(object), typeof(IBeforeSaveChangeEventHandler<>), false)]
        public void FindGenericInterface_DetectsImplementation(Type type, Type interfaceType, bool expectedResult)
        {
            var result = TypeHelpers.FindGenericInterface(type, interfaceType);

            Assert.Equal(expectedResult, result != null);
        }
        
        [Fact]
        public void EnumerateTypeHierarchy_Object_FindsOne()
        {
            var type = typeof(object);

            var result = TypeHelpers.EnumerateTypeHierarchy(type);

            Assert.Single(result);
            Assert.Equal(type, result.Single());
        }
    }
}
