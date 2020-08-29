using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class AfterSaveFailedTriggerDescriptorTests
    {
        [Fact]
        public void TriggerType_ReturnsConstructuredTriggerType()
        {
            var entityType = typeof(string);
            var exception = new Exception();
            var subject = new AfterSaveFailedTriggerDescriptor(entityType, exception);

            Assert.Equal(typeof(IAfterSaveFailedTrigger<string>), subject.TriggerType);
        }

        [Fact]
        public async Task Execute_ForwardsCall()
        {
            var entityType = typeof(string);
            var exception = new Exception();
            var triggerStub = new TriggerStub<string>();
            var subject = new AfterSaveFailedTriggerDescriptor(entityType, exception);

            await subject.Invoke(triggerStub, new TriggerContextStub<string>(), exception, default);

            Assert.Single(triggerStub.AfterSaveFailedInvocations);
            Assert.Equal(exception, triggerStub.AfterSaveFailedInvocations.First().exception);
        }
    }
}
