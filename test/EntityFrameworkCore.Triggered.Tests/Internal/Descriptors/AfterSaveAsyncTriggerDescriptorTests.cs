using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal.Descriptors;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal.Descriptors
{
    public class AfterSaveAsyncTriggerDescriptorTests
    {
        [Fact]
        public void TriggerType_ReturnsConstructuredTriggerType()
        {
            var entityType = typeof(string);
            var subject = new AfterSaveAsyncTriggerDescriptor(entityType);

            Assert.Equal(typeof(IAfterSaveAsyncTrigger<string>), subject.TriggerType);
        }

        [Fact]
        public async Task Execute_ForwardsCall()
        {
            var entityType = typeof(string);
            var triggerStub = new TriggerStub<string>();
            var subject = new AfterSaveAsyncTriggerDescriptor(entityType);

            await subject.Invoke(triggerStub, new TriggerContextStub<string>(), null, default);

            Assert.Single(triggerStub.AfterSaveInvocations);
        }
    }
}
