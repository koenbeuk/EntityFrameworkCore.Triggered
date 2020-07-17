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
    public class BeforeSaveTriggerAdapterTests
    {
        [Fact]
        public async Task Execute_ForwardsCall()
        {
            var changeHandler = new TriggerStub<object>();
            var subject = new BeforeSaveTriggerAdapter(changeHandler);

            await subject.Execute(new TriggerContextStub<object> { }, default);

            Assert.Single(changeHandler.BeforeSaveInvocations);
        }
    }
}
