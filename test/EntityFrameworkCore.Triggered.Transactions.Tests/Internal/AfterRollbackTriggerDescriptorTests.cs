using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Transactions.Internal;
using EntityFrameworkCore.Triggered.Transactions.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Transactions.Tests.Internal
{
    public class AfterRollbackTriggerDescriptorTests
    {
        [Fact]
        public void TriggerType_ReturnsConstructuredTriggerType()
        {
            var entityType = typeof(string);
            var subject = new AfterRollbackTriggerDescriptor(entityType);

            Assert.Equal(typeof(IAfterRollbackTrigger<string>), subject.TriggerType);
        }

        [Fact]
        public async Task Execute_ForwardsCall()
        {
            var entityType = typeof(string);
            var triggerStub = new TriggerStub<string>();
            var subject = new AfterRollbackTriggerDescriptor(entityType);

            await subject.Invoke(triggerStub, new TriggerContextStub<string>(), null, default);

            Assert.Single(triggerStub.AfterRollbackInvocations);
        }
    }
}
