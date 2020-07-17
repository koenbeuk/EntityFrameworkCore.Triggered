using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Internal;
using EntityFrameworkCore.Triggers.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests.Internal
{
    public class AfterSaveTriggerAdapterTests
    {
        [Fact]
        public async Task Execute_ForwardsCall()
        {
            var changeHandler = new TriggerStub<object>();
            var subject = new AfterSaveTriggerAdapter(changeHandler);

            await subject.Execute(new TriggerContextStub<object> { }, default);

            Assert.Single(changeHandler.AfterSaveInvocations);
        }
    }
}
