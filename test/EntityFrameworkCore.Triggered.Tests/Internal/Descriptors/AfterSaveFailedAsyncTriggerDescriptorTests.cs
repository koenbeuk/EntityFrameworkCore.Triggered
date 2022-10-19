using System;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal.Descriptors;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal.Descriptors
{
    public class AfterSaveFailedAsyncTriggerDescriptorTests
    {
        [Fact]
        public void TriggerType_ReturnsConstructuredTriggerType()
        {
            var entityType = typeof(string);
            var subject = new AfterSaveFailedAsyncTriggerDescriptor(entityType);

            Assert.Equal(typeof(IAfterSaveFailedAsyncTrigger<string>), subject.TriggerType);
        }

        [Fact]
        public async Task Execute_ForwardsCall()
        {
            var entityType = typeof(string);
            var exception = new Exception();
            var triggerStub = new TriggerStub<string>();
            var subject = new AfterSaveFailedAsyncTriggerDescriptor(entityType);

            await subject.Invoke(triggerStub, new TriggerContextStub<string>(), exception, default);

            Assert.Single(triggerStub.AfterSaveFailedAsyncInvocations);
            Assert.Equal(exception, triggerStub.AfterSaveFailedAsyncInvocations.First().exception);
        }
    }
}
