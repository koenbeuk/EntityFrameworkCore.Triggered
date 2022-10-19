using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerTypeDescriptorHelpersTests
    {
        [Fact]
        public async Task CreateWeakDelegate_WrapsTargetMethod()
        {
            var triggerStub = new TriggerStub<object>();
            var triggerContextStub = new TriggerContextStub<object>();
            var weakDelegate = TriggerTypeDescriptorHelpers.GetAsyncWeakDelegate(typeof(IBeforeSaveTrigger<object>), typeof(object), typeof(IBeforeSaveTrigger<object>).GetMethod("BeforeSave"));

            await weakDelegate(triggerStub, triggerContextStub, default);

            Assert.NotEmpty(triggerStub.BeforeSaveInvocations);
        }
    }
}
