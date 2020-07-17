using System;
using System.Collections.Generic;
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

            var registry = new TriggerRegistry(typeof(IBeforeSaveTrigger<>), serviceProvider, x => new TriggerAdapterStub(x));

            var result = registry.DiscoverChangeHandlers(typeof(string));
            Assert.Single(result);
        }

        [Fact]
        public void DiscoverChangeHandlerInvocations_BaseType_CreatesInvocation()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IBeforeSaveTrigger<object>, TriggerStub<object>>()
                .BuildServiceProvider();

            var registry = new TriggerRegistry(typeof(IBeforeSaveTrigger<>), serviceProvider, x => new TriggerAdapterStub(x));

            var result = registry.DiscoverChangeHandlers(typeof(string));
            Assert.Single(result);
        }

        [Fact]
        public void DiscoverChangeHandlerInvocations_InterfaceType_CreatesInvocation()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IBeforeSaveTrigger<object>, TriggerStub<object>>()
                .BuildServiceProvider();

            var registry = new TriggerRegistry(typeof(IBeforeSaveTrigger<>), serviceProvider, x => new TriggerAdapterStub(x));

            var result = registry.DiscoverChangeHandlers(typeof(string));
            Assert.Single(result);
        }
    }
}
