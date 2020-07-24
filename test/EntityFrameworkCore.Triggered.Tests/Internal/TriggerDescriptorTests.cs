using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerDescriptorTests 
    {
        class TestTriggerWithPriority : IBeforeSaveTrigger<object>, ITriggerPriority
        {
            public int Priority => 1;

            public Task BeforeSave(ITriggerContext<object> context, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        class TestTriggerWithoutPriority : IBeforeSaveTrigger<object>
        {
            public Task BeforeSave(ITriggerContext<object> context, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void Priority_TriggerWithoutPriority_ReturnsDefault()
        {
            var subject = new TriggerDescriptor(new BeforeSaveTriggerDescriptor(typeof(object)), new TestTriggerWithoutPriority());
            Assert.Equal(0, subject.Priority);
        }

        [Fact]
        public void Priority_TriggerWithPriority_ReturnsPriority()
        {
            var subject = new TriggerDescriptor(new BeforeSaveTriggerDescriptor(typeof(object)), new TestTriggerWithPriority());
            Assert.Equal(1, subject.Priority);
        }
    }
}
