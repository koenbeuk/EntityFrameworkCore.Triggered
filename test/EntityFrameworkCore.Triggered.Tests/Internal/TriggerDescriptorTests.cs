using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal;

public class TriggerDescriptorTests
{
    class TestTriggerWithPriority : IBeforeSaveTrigger<object>, ITriggerPriority
    {
        public int Priority => 1;

        public void BeforeSave(ITriggerContext<object> context) => throw new NotImplementedException();
    }

    class TestTriggerWithoutPriority : IBeforeSaveTrigger<object>
    {
        public void BeforeSave(ITriggerContext<object> context) => throw new NotImplementedException();
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
