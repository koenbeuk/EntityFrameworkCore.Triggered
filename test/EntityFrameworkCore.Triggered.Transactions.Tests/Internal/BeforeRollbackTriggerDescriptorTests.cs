using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Transactions.Internal;
using EntityFrameworkCore.Triggered.Transactions.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Transactions.Tests.Internal
{
    public class BeforeRollbackTriggerDescriptorTests
    {
        [Fact]
        public void TriggerType_ReturnsConstructuredTriggerType()
        {
            var entityType = typeof(string);
            var subject = new BeforeRollbackTriggerDescriptor(entityType);

            Assert.Equal(typeof(IBeforeRollbackTrigger<string>), subject.TriggerType);
        }

        [Fact]
        public async Task Execute_ForwardsCall()
        {
            var entityType = typeof(string);
            var triggerStub = new TriggerStub<string>();
            var subject = new BeforeRollbackTriggerDescriptor(entityType);

            await subject.Invoke(triggerStub, new TriggerContextStub<string>(), null, default);

            Assert.Single(triggerStub.BeforeRollbackInvocations);
        }
    }
}
