using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerRegistryTests
    {
        [Fact]
        public void DiscoverChangeHandlerInvocations_ConcreteType_CreatesInvocation()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IBeforeSaveTrigger<object>, TriggerStub<object>>()
                .BuildServiceProvider();

            var registry = new TriggerRegistry(typeof(IBeforeSaveTrigger<>), serviceProvider, null, x => new TriggerAdapterStub(x));

            var result = registry.DiscoverTriggers(typeof(string));
            Assert.Single(result);
        }

        [Fact]
        public void DiscoverChangeHandlerInvocations_BaseType_CreatesInvocation()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IBeforeSaveTrigger<object>, TriggerStub<object>>()
                .BuildServiceProvider();

            var registry = new TriggerRegistry(typeof(IBeforeSaveTrigger<>), serviceProvider, null, x => new TriggerAdapterStub(x));

            var result = registry.DiscoverTriggers(typeof(string));
            Assert.Single(result);
        }

        [Fact]
        public void DiscoverChangeHandlerInvocations_InterfaceType_CreatesInvocation()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IBeforeSaveTrigger<object>, TriggerStub<object>>()
                .BuildServiceProvider();

            var registry = new TriggerRegistry(typeof(IBeforeSaveTrigger<>), serviceProvider, null, x => new TriggerAdapterStub(x));

            var result = registry.DiscoverTriggers(typeof(string));
            Assert.Single(result);
        }

        [Fact]
        public void DiscoverChangeHandlerInvocations_SortByPriorities()
        {
            var earlyTrigger = new TriggerStub<object> { Priority = CommonTriggerPriority.Early };
            var lateTrigger = new TriggerStub<object> { Priority = CommonTriggerPriority.Late };


            var serviceProvider = new ServiceCollection()
                .AddSingleton<IBeforeSaveTrigger<object>>(lateTrigger)
                .AddSingleton<IBeforeSaveTrigger<object>>(earlyTrigger)
                .BuildServiceProvider();

            var registry = new TriggerRegistry(typeof(IBeforeSaveTrigger<>), serviceProvider, null, x => new TriggerAdapterStub(x));

            var result = registry.DiscoverTriggers(typeof(string));
            Assert.Equal(2, result.Count());
            Assert.Equal(earlyTrigger, result.First().Trigger);
            Assert.Equal(lateTrigger, result.Last().Trigger);
        }
    }
}
