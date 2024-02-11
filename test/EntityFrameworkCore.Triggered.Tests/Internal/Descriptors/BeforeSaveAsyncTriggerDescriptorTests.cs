using EntityFrameworkCore.Triggered.Internal.Descriptors;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal.Descriptors
{
    public class BeforeSaveAsyncTriggerDescriptorTests
    {
        [Fact]
        public void TriggerType_ReturnsConstructuredTriggerType()
        {
            var entityType = typeof(string);
            var subject = new BeforeSaveAsyncTriggerDescriptor(entityType);

            Assert.Equal(typeof(IBeforeSaveAsyncTrigger<string>), subject.TriggerType);
        }

        [Fact]
        public async Task Invoke_ForwardsCall()
        {
            var entityType = typeof(string);
            var triggerStub = new TriggerStub<string>();
            var subject = new BeforeSaveAsyncTriggerDescriptor(entityType);

            await subject.Invoke(triggerStub, new TriggerContextStub<string>(), null, default);

            Assert.Single(triggerStub.BeforeSaveAsyncInvocations);
        }
    }
}
