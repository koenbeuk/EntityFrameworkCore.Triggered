using EntityFrameworkCore.Triggered.Internal.Descriptors;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal.Descriptors
{
    public class AfterSaveTriggerDescriptorTests
    {
        [Fact]
        public void TriggerType_ReturnsConstructuredTriggerType()
        {
            var entityType = typeof(string);
            var subject = new AfterSaveTriggerDescriptor(entityType);

            Assert.Equal(typeof(IAfterSaveTrigger<string>), subject.TriggerType);
        }

        [Fact]
        public void Execute_ForwardsCall()
        {
            var entityType = typeof(string);
            var triggerStub = new TriggerStub<string>();
            var subject = new AfterSaveTriggerDescriptor(entityType);

            subject.Invoke(triggerStub, new TriggerContextStub<string>(), null);

            Assert.Single(triggerStub.AfterSaveInvocations);
        }
    }
}
