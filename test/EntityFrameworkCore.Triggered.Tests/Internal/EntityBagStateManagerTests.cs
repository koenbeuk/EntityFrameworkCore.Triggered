using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class EntityBagStateManagerTests
    {
        [Fact]
        public void GetForEntity_MultipleCallsForSameEntity_ReturnsSameInstance()
        {
            var subject = new EntityBagStateManager();
            var entity = new object();
            var expectedBag = subject.GetForEntity(entity);

            var bag = subject.GetForEntity(entity);

            Assert.Same(expectedBag, bag);
        }

        [Fact]
        public void GetForEntity_MultipleCallsForDifferentEntities_ReturnsDifferentInstances()
        {
            var subject = new EntityBagStateManager();
            var entity1 = new object();
            var entity2 = new object();

            var entity1Bag = subject.GetForEntity(entity1);
            var entity2Bag = subject.GetForEntity(entity2);

            Assert.NotSame(entity1Bag, entity2Bag);
        }
    }
}
