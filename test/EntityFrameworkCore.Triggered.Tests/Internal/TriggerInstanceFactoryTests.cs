using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScenarioTests;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public partial class TriggerInstanceFactoryTests
    {
        class SampleDbContext : DbContext
        {
            public SampleDbContext(DbContextOptions options) : base(options)
            {
            }
        }

        class SampleTrigger1 { }
        class SampleTrigger2 { public SampleTrigger2(SampleDbContext sampleDbContext) { } }

        [Scenario]
        public void TriggerInstanceFactoriesBehaviorTests(ScenarioContext scenario)
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<SampleDbContext>()
                .BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<SampleDbContext>();

            ITriggerInstanceFactory subject = new TriggerInstanceFactory<SampleTrigger1>(null);
            var instance = subject.Create(serviceProvider);

            scenario.Fact("Creates an instance on first request", () => {
                Assert.NotNull(instance);
            });

            scenario.Fact("Caches that instance on subsequent requests", () => {
                var secondInstance = subject.Create(serviceProvider);
                Assert.Equal(instance, secondInstance);
            });

            scenario.Fact("When provided with an initial instance, it will always return that", () => {
                subject = new TriggerInstanceFactory<SampleTrigger1>(instance);
                var secondInstance = subject.Create(serviceProvider);
                Assert.Equal(instance, secondInstance);
            });

            scenario.Fact("Gets provided the DbContext when its requested", () => {
                subject = new TriggerInstanceFactory<SampleTrigger2>(null);
                instance = subject.Create(serviceProvider);
                Assert.NotNull(instance);
            });
        }
    }
}
