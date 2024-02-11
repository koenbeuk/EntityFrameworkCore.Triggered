using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.Descriptors;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal;

public class TriggerDiscoveryServiceTests
{
    class TriggerServiceProviderAccessor(IServiceProvider serviceProvider) : ITriggerServiceProviderAccessor
    {
        readonly IServiceProvider _serviceProvider = serviceProvider;

        public IServiceProvider GetTriggerServiceProvider() => _serviceProvider;
    }

    [Fact]
    public void DiscoverTriggers_ConcreteType_CreatesDescriptor()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped<IBeforeSaveTrigger<string>, TriggerStub<string>>()
            .BuildServiceProvider();

        var subject = new TriggerDiscoveryService(new TriggerServiceProviderAccessor(serviceProvider), new TriggerTypeRegistryService(), new TriggerFactory(serviceProvider));

        var result = subject.DiscoverTriggers(typeof(IBeforeSaveTrigger<>), typeof(string), type => new BeforeSaveTriggerDescriptor(type));

        Assert.Single(result);
    }

    [Fact]
    public void DiscoverTriggers_BaseType_CreatesDescriptor()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped<IBeforeSaveTrigger<object>, TriggerStub<object>>()
            .BuildServiceProvider();

        var subject = new TriggerDiscoveryService(new TriggerServiceProviderAccessor(serviceProvider), new TriggerTypeRegistryService(), new TriggerFactory(serviceProvider));

        var result = subject.DiscoverTriggers(typeof(IBeforeSaveTrigger<>), typeof(string), type => new BeforeSaveTriggerDescriptor(type));

        Assert.Single(result);
    }

    [Fact]
    public void DiscoverChangeHandlerInvocations_InterfaceType_CreatesDescriptor()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped<IBeforeSaveTrigger<IComparable>, TriggerStub<IComparable>>()
            .BuildServiceProvider();

        var subject = new TriggerDiscoveryService(new TriggerServiceProviderAccessor(serviceProvider), new TriggerTypeRegistryService(), new TriggerFactory(serviceProvider));

        var result = subject.DiscoverTriggers(typeof(IBeforeSaveTrigger<>), typeof(string), type => new BeforeSaveTriggerDescriptor(type));

        Assert.Single(result);
    }

    [Fact]
    public void DiscoverChangeHandlerInvocations_SortTriggersByInterfaceThenType()
    {
        var interfaceTrigger = new TriggerStub<IComparable>();
        var typeTrigger = new TriggerStub<string>();

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IBeforeSaveTrigger<string>>(typeTrigger)
            .AddSingleton<IBeforeSaveTrigger<IComparable>>(interfaceTrigger)
            .BuildServiceProvider();

        var subject = new TriggerDiscoveryService(new TriggerServiceProviderAccessor(serviceProvider), new TriggerTypeRegistryService(), new TriggerFactory(serviceProvider));

        var result = subject.DiscoverTriggers(typeof(IBeforeSaveTrigger<>), typeof(string), type => new BeforeSaveTriggerDescriptor(type));

        Assert.Equal(2, result.Count());
        Assert.Equal(interfaceTrigger, result.First().Trigger);
        Assert.Equal(typeTrigger, result.Last().Trigger);
    }

    [Fact]
    public void DiscoverChangeHandlerInvocations_SortTriggersByBaseTypeThenDerivedType()
    {
        var objectTrigger = new TriggerStub<object>();
        var concreteTrigger = new TriggerStub<string>();

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IBeforeSaveTrigger<string>>(concreteTrigger)
            .AddSingleton<IBeforeSaveTrigger<object>>(objectTrigger)
            .BuildServiceProvider();

        var subject = new TriggerDiscoveryService(new TriggerServiceProviderAccessor(serviceProvider), new TriggerTypeRegistryService(), new TriggerFactory(serviceProvider));

        var result = subject.DiscoverTriggers(typeof(IBeforeSaveTrigger<>), typeof(string), type => new BeforeSaveTriggerDescriptor(type));

        Assert.Equal(2, result.Count());
        Assert.Equal(objectTrigger, result.First().Trigger);
        Assert.Equal(concreteTrigger, result.Last().Trigger);
    }

    [Fact]
    public void DiscoverChangeHandlerInvocations_SortTriggersByBaseTypeThenInterfacesThenDerivedType()
    {
        var interfaceTrigger = new TriggerStub<IComparable>();
        var objectTrigger = new TriggerStub<object>();
        var concreteTrigger = new TriggerStub<string>();

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IBeforeSaveTrigger<IComparable>>(interfaceTrigger)
            .AddSingleton<IBeforeSaveTrigger<string>>(concreteTrigger)
            .AddSingleton<IBeforeSaveTrigger<object>>(objectTrigger)
            .BuildServiceProvider();

        var subject = new TriggerDiscoveryService(new TriggerServiceProviderAccessor(serviceProvider), new TriggerTypeRegistryService(), new TriggerFactory(serviceProvider));

        var result = subject.DiscoverTriggers(typeof(IBeforeSaveTrigger<>), typeof(string), type => new BeforeSaveTriggerDescriptor(type));

        Assert.Equal(3, result.Count());
        Assert.Equal(objectTrigger, result.First().Trigger);
        Assert.Equal(interfaceTrigger, result.Skip(1).First().Trigger);
        Assert.Equal(concreteTrigger, result.Last().Trigger);
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

        var subject = new TriggerDiscoveryService(new TriggerServiceProviderAccessor(serviceProvider), new TriggerTypeRegistryService(), new TriggerFactory(serviceProvider));

        var result = subject.DiscoverTriggers(typeof(IBeforeSaveTrigger<>), typeof(string), type => new BeforeSaveTriggerDescriptor(type));

        Assert.Equal(2, result.Count());
        Assert.Equal(earlyTrigger, result.First().Trigger);
        Assert.Equal(lateTrigger, result.Last().Trigger);
    }

    [Fact]
    public void SetServiceProvider_OverridesDefaultServiceProvider()
    {
        var defaultServiceProviderTrigger = new TriggerStub<object>();
        var externalServiceProviderTrigger = new TriggerStub<object>();

        var defaultServiceProvider = new ServiceCollection()
            .BuildServiceProvider();

        var externalServiceProvider = new ServiceCollection()
            .AddSingleton<IBeforeSaveTrigger<object>>(externalServiceProviderTrigger)
            .BuildServiceProvider();

        var subject = new TriggerDiscoveryService(new TriggerServiceProviderAccessor(defaultServiceProvider), new TriggerTypeRegistryService(), new TriggerFactory(defaultServiceProvider));
        subject.ServiceProvider = externalServiceProvider;

        var result = subject.DiscoverTriggers(typeof(IBeforeSaveTrigger<>), typeof(string), type => new BeforeSaveTriggerDescriptor(type));

        Assert.Single(result);
    }
}
