using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Lyfecycles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.Extensions.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        public static IEnumerable<object[]> Lifetimes()
        {
            yield return new object[] { ServiceLifetime.Transient };
            yield return new object[] { ServiceLifetime.Scoped };
            yield return new object[] { ServiceLifetime.Singleton };
        }

        public static IEnumerable<object[]> TriggerTypes()
        {
            yield return new object[] { typeof(IBeforeSaveTrigger<object>) };
            yield return new object[] { typeof(IAfterSaveTrigger<object>) };
            yield return new object[] { typeof(IAfterSaveFailedTrigger<object>) };
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddTriggerOfT_WithCustomLifetime_RegistersWithThatLifetime(ServiceLifetime lifetime)
        {
            var serviceCollection = new ServiceCollection()
                .AddTrigger<Trigger<object>>(lifetime);

            Assert.Equal(lifetime, serviceCollection.First().Lifetime);
        }

        [Theory]
        [MemberData(nameof(TriggerTypes))]
        public void AddTriggerOfT_RegistersAllLifetimes(Type triggerLifetimeType)
        {
            var serviceCollection = new ServiceCollection()
                .AddTrigger<Trigger<object>>();

            Assert.Contains(serviceCollection, x => x.ServiceType == triggerLifetimeType);
        }

        [Theory]
        [MemberData(nameof(TriggerTypes))]
        public void AddTriggerInstance_RegistersAllTypes(Type triggerLifetimeType)
        {
            var serviceCollection = new ServiceCollection()
                .AddTrigger(new Trigger<object>());

            Assert.Contains(serviceCollection, x => x.ServiceType == triggerLifetimeType);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddAssemblyTriggers_WithCustomLifetime_RegistersWithThatLifetime(ServiceLifetime lifetime)
        {
            var serviceCollection = new ServiceCollection()
                .AddTrigger<Trigger<object>>(lifetime);

            Assert.Equal(lifetime, serviceCollection.First().Lifetime);
        }

        [Theory]
        [MemberData(nameof(TriggerTypes))]
        public void AddAssemblyTriggers_WithType_RegistersWithThatType(Type triggerLifetimeType)
        {
            var serviceCollection = new ServiceCollection()
                .AddTrigger<Trigger<object>>();

            Assert.Contains(serviceCollection, x => x.ServiceType == triggerLifetimeType);
        }
    }
}
