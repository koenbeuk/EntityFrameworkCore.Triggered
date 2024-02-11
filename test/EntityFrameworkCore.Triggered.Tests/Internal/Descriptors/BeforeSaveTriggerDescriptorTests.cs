using EntityFrameworkCore.Triggered.Internal.Descriptors;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal.Descriptors
{
    public class BeforeSaveTriggerDescriptorTests
    {
        [Fact]
        public void TriggerType_ReturnsConstructuredTriggerType()
        {
            var entityType = typeof(string);
            var subject = new BeforeSaveTriggerDescriptor(entityType);

            Assert.Equal(typeof(IBeforeSaveTrigger<string>), subject.TriggerType);
        }

        [Fact]
        public void Invoke_ForwardsCall()
        {
            var entityType = typeof(string);
            var triggerStub = new TriggerStub<string>();
            var subject = new BeforeSaveTriggerDescriptor(entityType);

            subject.Invoke(triggerStub, new TriggerContextStub<string>(), null);

            Assert.Single(triggerStub.BeforeSaveInvocations);
        }
    }
}
