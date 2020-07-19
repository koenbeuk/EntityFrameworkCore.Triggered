using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TypeHelpersTests
    {
        [Theory]
        [InlineData(typeof(List<>), typeof(ICollection<>), true)]
        [InlineData(typeof(List<object>), typeof(ICollection<>), true)]
        [InlineData(typeof(object), typeof(IBeforeSaveTrigger<>), false)]
        public void FindGenericInterface_DetectsImplementation(Type type, Type interfaceType, bool expectedResult)
        {
            var result = TypeHelpers.FindGenericInterfaces(type, interfaceType).Any();

            Assert.Equal(expectedResult, result);
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
