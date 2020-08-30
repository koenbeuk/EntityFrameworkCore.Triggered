using EntityFrameworkCore.Triggered.Internal;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerTypeRegistryServiceTests
    {
        [Fact]
        public void ResolveRegistry_SameTypePattern_ReturnsSameInstance()
        {
            var subject = new TriggerTypeRegistryService();
            var triggerType = typeof(IBeforeSaveTrigger<>);
            var entityType = typeof(object);

            var registry1 = subject.ResolveRegistry(triggerType, entityType, _ => null);
            var registry2 = subject.ResolveRegistry(triggerType, entityType, _ => null);

            Assert.Equal(registry1, registry2);
        }

        [Fact]
        public void ResolveRegistry_DifferentEntityType_ReturnsSameInstance()
        {
            var subject = new TriggerTypeRegistryService();
            var triggerType = typeof(IBeforeSaveTrigger<>);

            var registry1 = subject.ResolveRegistry(triggerType, typeof(object), _ => null);
            var registry2 = subject.ResolveRegistry(triggerType, typeof(string), _ => null);

            Assert.NotEqual(registry1, registry2);
        }

        [Fact]
        public void ResolveRegistry_DifferentTriggerType_ReturnsDifferentInstance()
        {
            var subject = new TriggerTypeRegistryService();
            var entityType = typeof(object);

            var registry1 = subject.ResolveRegistry(typeof(IBeforeSaveTrigger<>), entityType, _ => null);
            var registry2 = subject.ResolveRegistry(typeof(IAfterSaveTrigger<>), entityType, _ => null);

            Assert.NotEqual(registry1, registry2);
        }
    }
}
