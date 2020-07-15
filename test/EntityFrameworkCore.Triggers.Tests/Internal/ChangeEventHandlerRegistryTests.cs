using System;
using System.Collections.Generic;
using System.Text;
using EntityFrameworkCore.Triggers.Internal;
using EntityFrameworkCore.Triggers.Tests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests.Internal
{
    public class ChangeEventHandlerRegistryTests
    {
        [Fact]
        public void DiscoverChangeHandlerInvocations_ConcreteType_CreatesInvocation()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IBeforeSaveChangeEventHandler<object>, ChangeEventHandlerStub<object>>()
                .BuildServiceProvider();

            var registry = new ChangeEventHandlerRegistry(typeof(IBeforeSaveChangeEventHandler<>), serviceProvider, x => new ChangeEventHandlerEventExecutionStrategyStub(x));

            var result = registry.DiscoverChangeHandlers(typeof(string));
            Assert.Single(result);
        }

        [Fact]
        public void DiscoverChangeHandlerInvocations_BaseType_CreatesInvocation()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IBeforeSaveChangeEventHandler<object>, ChangeEventHandlerStub<object>>()
                .BuildServiceProvider();

            var registry = new ChangeEventHandlerRegistry(typeof(IBeforeSaveChangeEventHandler<>), serviceProvider, x => new ChangeEventHandlerEventExecutionStrategyStub(x));

            var result = registry.DiscoverChangeHandlers(typeof(string));
            Assert.Single(result);
        }

        [Fact]
        public void DiscoverChangeHandlerInvocations_InterfaceType_CreatesInvocation()
        {
            var serviceProvider = new ServiceCollection()
                .AddScoped<IBeforeSaveChangeEventHandler<object>, ChangeEventHandlerStub<object>>()
                .BuildServiceProvider();

            var registry = new ChangeEventHandlerRegistry(typeof(IBeforeSaveChangeEventHandler<>), serviceProvider, x => new ChangeEventHandlerEventExecutionStrategyStub(x));

            var result = registry.DiscoverChangeHandlers(typeof(string));
            Assert.Single(result);
        }
    }
}
