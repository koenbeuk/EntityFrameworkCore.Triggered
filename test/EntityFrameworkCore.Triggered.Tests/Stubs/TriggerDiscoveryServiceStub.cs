using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerDiscoveryServiceStub : ITriggerDiscoveryService
    {
        public IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory)
            => Enumerable.Empty<TriggerDescriptor>();
        public IEnumerable<TTrigger> DiscoverTriggers<TTrigger>()
            => Enumerable.Empty<TTrigger>();

        public IServiceProvider ServiceProvider { get; set; } = new ServiceCollection().BuildServiceProvider();
    }
}
