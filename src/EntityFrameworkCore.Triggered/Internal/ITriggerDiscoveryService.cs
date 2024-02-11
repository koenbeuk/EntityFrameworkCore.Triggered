using EntityFrameworkCore.Triggered.Internal.Descriptors;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerDiscoveryService
    {
        IEnumerable<AsyncTriggerDescriptor> DiscoverAsyncTriggers(Type openTriggerType, Type entityType, Func<Type, IAsyncTriggerTypeDescriptor> triggerTypeDescriptorFactory);

        IEnumerable<TriggerDescriptor> DiscoverTriggers(Type openTriggerType, Type entityType, Func<Type, ITriggerTypeDescriptor> triggerTypeDescriptorFactory);

        IEnumerable<TTrigger> DiscoverTriggers<TTrigger>();

        public IServiceProvider ServiceProvider { get; set; }
    }
}
