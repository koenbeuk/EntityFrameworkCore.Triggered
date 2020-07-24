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
        public async Task Execute_ForwardsCall()
        {
            var entityType = typeof(string);
            var triggerStub = new TriggerStub<string>();
            var subject = new AfterSaveTriggerDescriptor(entityType);

            await subject.Invoke(triggerStub, new TriggerContextStub<string>(), default);

            Assert.Single(triggerStub.AfterSaveInvocations);
        }
    }
}
