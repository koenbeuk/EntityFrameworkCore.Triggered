using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        public class TestModel
        {
            public int Id { get; set; }
        }

        public class TestDbContext :
#if EFCORETRIGGERED2 || EFCORETRIGGERED3
            DbContext
#else
            TriggeredDbContext
#endif
        {
            public TestDbContext(DbContextOptions options)
                : base(options)
            {
            }

            public DbSet<TestModel> TestModels { get; set; }
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
                .AddAssemblyTriggers(lifetime);

            Assert.Equal(lifetime, serviceCollection.First().Lifetime);
        }

        [Fact]
        public void AddAssemblyTriggers_WithAssembly_RegistersWithThatAssembly()
        {
            var serviceCollection = new ServiceCollection()
                .AddAssemblyTriggers(typeof(ServiceCollectionExtensionsTests).Assembly);

            Assert.Equal(5, serviceCollection.Count);
        }

        protected async Task SaveChanges_TriggeredAddedThroughDI_Template(Func<IServiceCollection, IServiceCollection> transform)
        {
            var serviceProvider = transform(new ServiceCollection()
                .AddTriggeredDbContext<TestDbContext>(options => {
                    options.UseInMemoryDatabase("test");
                    options.ConfigureWarnings(warningOptions => {
                        warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                    });
                }))
                .BuildServiceProvider();

            using var dbContext = serviceProvider.GetRequiredService<TestDbContext>();
            dbContext.TestModels.Add(new TestModel { });

            await dbContext.SaveChangesAsync();

            var trigger = serviceProvider.GetRequiredService<SampleTrigger>();

            Assert.Equal(1, trigger.BeforeSaveStartingTriggerCalls);
            Assert.Equal(1, trigger.BeforeSaveCalls);
            Assert.Equal(1, trigger.BeforeSaveAsyncCalls);
            Assert.Equal(1, trigger.AfterSaveCalls);
            Assert.Equal(1, trigger.AfterSaveAsyncCalls);
        }

        [Fact]
        public Task SaveChanges_ExplicitlyAddedTriggerThroughDI_RaisesAllTriggerTypes()
            => SaveChanges_TriggeredAddedThroughDI_Template(x => x.AddTrigger<SampleTrigger>());


        [Fact]
        public Task SaveChanges_DiscoveredTriggerThroughDI_RaisesAllTriggerTypes()
            => SaveChanges_TriggeredAddedThroughDI_Template(x => x.AddAssemblyTriggers());

        [Fact]
        public void AddAssemblyTriggers_AbstractTrigger_GetsIgnored()
        {
            var serviceCollection = new ServiceCollection()
                .AddAssemblyTriggers();

            // Ensure that we did not register the AbstractTrigger
            Assert.Empty(serviceCollection.Where(x => x.ImplementationType == typeof(AbstractTrigger)));
        }
    }
}
