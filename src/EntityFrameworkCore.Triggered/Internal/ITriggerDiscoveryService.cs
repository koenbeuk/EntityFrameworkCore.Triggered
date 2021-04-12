using System;
using System.Collections.Generic;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerDiscoveryService
    {
        IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory);

        IEnumerable<TTrigger> DiscoverTriggers<TTrigger>();

        public IServiceProvider ServiceProvider { get; set; }
    }
}
