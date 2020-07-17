using System;
using System.Collections.Generic;
using System.Text;
using EntityFrameworkCore.Triggers.Internal;
using EntityFrameworkCore.Triggers.Tests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests.Internal
{
    public class TriggerRegistryServiceTests
    {
        [Fact]
        public void GetRegistry_SameType_ReturnsSameRegistry()
        {
            var serviceProvider = new ServiceCollection()
                .BuildServiceProvider();

            var registryService = new TriggerRegistryService(serviceProvider);

            var registry1 = registryService.GetRegistry(typeof(IBeforeSaveTrigger<>), _ => null);
            var registry2 = registryService.GetRegistry(typeof(IBeforeSaveTrigger<>), _ => null);

            Assert.Equal(registry1, registry2);
        }

        [Fact]
        public void GetRegistry_DifferentType_ReturnsSameRegistry()
        {
            var serviceProvider = new ServiceCollection()
                .BuildServiceProvider();

            var registryService = new TriggerRegistryService(serviceProvider);

            var registry1 = registryService.GetRegistry(typeof(IBeforeSaveTrigger<>), _ => null);
            var registry2 = registryService.GetRegistry(typeof(IAfterSaveTrigger<>), _ => null);

            Assert.NotEqual(registry1, registry2);
        }
    }
}
