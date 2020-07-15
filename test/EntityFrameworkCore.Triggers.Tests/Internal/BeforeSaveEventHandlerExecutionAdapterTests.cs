using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Internal;
using EntityFrameworkCore.Triggers.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests.Internal
{
    public class BeforeSaveEventHandlerExecutionAdapterTests
    {
        [Fact]
        public async Task Execute_ForwardsCall()
        {
            var changeHandler = new ChangeEventHandlerStub<object>();
            var subject = new BeforeSaveEventHandlerAdapter(changeHandler);

            await subject.Execute(new ChangeEventStub<object> { }, default);

            Assert.Single(changeHandler.BeforeSaveInvocations);
        }
    }
}
