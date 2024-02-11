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
        public void Execute_ForwardsCall()
        {
            var entityType = typeof(string);
            var triggerStub = new TriggerStub<string>();
            var subject = new AfterRollbackTriggerDescriptor(entityType);

            subject.Invoke(triggerStub, new TriggerContextStub<string>(), null);

            Assert.Single(triggerStub.AfterRollbackInvocations);
        }
    }
}
