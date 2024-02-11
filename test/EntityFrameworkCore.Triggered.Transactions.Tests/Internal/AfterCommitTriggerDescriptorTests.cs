using EntityFrameworkCore.Triggered.Transactions.Internal;
using EntityFrameworkCore.Triggered.Transactions.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Transactions.Tests.Internal;

public class AfterCommitTriggerDescriptorTests
{
    [Fact]
    public void TriggerType_ReturnsConstructuredTriggerType()
    {
        var entityType = typeof(string);
        var subject = new AfterCommitTriggerDescriptor(entityType);

        Assert.Equal(typeof(IAfterCommitTrigger<string>), subject.TriggerType);
    }

    [Fact]
    public void Execute_ForwardsCall()
    {
        var entityType = typeof(string);
        var triggerStub = new TriggerStub<string>();
        var subject = new AfterCommitTriggerDescriptor(entityType);

        subject.Invoke(triggerStub, new TriggerContextStub<string>(), null);

        Assert.Single(triggerStub.AfterCommitInvocations);
    }
}
