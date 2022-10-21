using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerTypeDescriptorHelpersTests
    {
        [Fact]
        public void CreateWeakDelegate_WrapsTargetMethod()
        {
            var triggerStub = new TriggerStub<object>();
            var triggerContextStub = new TriggerContextStub<object>();
            var weakDelegate = TriggerTypeDescriptorHelpers.GetWeakDelegate(typeof(IBeforeSaveTrigger<object>), typeof(object), typeof(IBeforeSaveTrigger<object>).GetMethod("BeforeSave"));

            weakDelegate(triggerStub, triggerContextStub);

            Assert.NotEmpty(triggerStub.BeforeSaveInvocations);
        }

        [Fact]
        public async Task CreateAsyncWeakDelegate_WrapsTargetMethod()
        {
            var triggerStub = new TriggerStub<object>();
            var triggerContextStub = new TriggerContextStub<object>();
            var weakDelegate = TriggerTypeDescriptorHelpers.GetAsyncWeakDelegate(typeof(IBeforeSaveAsyncTrigger<object>), typeof(object), typeof(IBeforeSaveAsyncTrigger<object>).GetMethod("BeforeSaveAsync"));

            await weakDelegate(triggerStub, triggerContextStub, default);

            Assert.NotEmpty(triggerStub.BeforeSaveAsyncInvocations);
        }
    }
}
