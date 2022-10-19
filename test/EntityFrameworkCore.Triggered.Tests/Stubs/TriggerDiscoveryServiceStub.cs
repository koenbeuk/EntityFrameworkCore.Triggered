using System;
using System.Collections.Generic;
using System.Linq;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerDiscoveryServiceStub : ITriggerDiscoveryService
    {
        public IEnumerable<AsyncTriggerDescriptor> DiscoverAsyncTriggers(Type openTriggerType, Type entityType, Func<Type, IAsyncTriggerTypeDescriptor> triggerTypeDescriptorFactory)
            => Enumerable.Empty<AsyncTriggerDescriptor>();

        public IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory) 
            => Enumerable.Empty<TriggerDescriptor>();

        public IEnumerable<TTrigger> DiscoverTriggers<TTrigger>()
            => Enumerable.Empty<TTrigger>();


        public IServiceProvider ServiceProvider { get; set; } = new ServiceCollection().BuildServiceProvider();
    }
}
