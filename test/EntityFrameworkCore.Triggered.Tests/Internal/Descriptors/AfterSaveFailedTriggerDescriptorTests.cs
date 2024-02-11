using System;
using System.Linq;
using EntityFrameworkCore.Triggered.Internal.Descriptors;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal.Descriptors
{
    public class AfterSaveFailedTriggerDescriptorTests
    {
        [Fact]
        public void TriggerType_ReturnsConstructuredTriggerType()
        {
            var entityType = typeof(string);
            var subject = new AfterSaveFailedTriggerDescriptor(entityType);

            Assert.Equal(typeof(IAfterSaveFailedTrigger<string>), subject.TriggerType);
        }

        [Fact]
        public void Execute_ForwardsCall()
        {
            var entityType = typeof(string);
            var exception = new Exception();
            var triggerStub = new TriggerStub<string>();
            var subject = new AfterSaveFailedTriggerDescriptor(entityType);

            subject.Invoke(triggerStub, new TriggerContextStub<string>(), exception);

            Assert.Single(triggerStub.AfterSaveFailedInvocations);
            Assert.Equal(exception, triggerStub.AfterSaveFailedInvocations.First().exception);
        }
    }
}
