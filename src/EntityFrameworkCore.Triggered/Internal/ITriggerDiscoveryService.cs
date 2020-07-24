using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerDiscoveryService
    {
        IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory);
    }
}
