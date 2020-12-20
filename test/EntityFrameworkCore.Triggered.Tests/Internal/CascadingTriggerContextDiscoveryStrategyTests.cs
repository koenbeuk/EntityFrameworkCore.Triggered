using System;
using System.Linq;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.CascadingStrategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class CascadeTriggerContextDiscoveryStrategyTests
    {
        class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions options) : base(options)
            {
            }

            public TestDbContext()
            {
            }

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseTriggers();
                optionsBuilder.ConfigureWarnings(warningOptions => {
                    warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                });
            }
        }


        [Fact]
        public void DiscoverChanges_MultipleCalls_ReturnsDeltaOfChanges()
        {
            using var dbContext = new TestDbContext();
            var subject = new CascadingTriggerContextDiscoveryStrategy("test", false);
            var triggerContextTracker = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeCascadingStrategy());
            triggerContextTracker.DiscoverChanges().Count();
            var initialContextDescriptors = subject.Discover(new TriggerOptions { }, triggerContextTracker, new NullLogger<object>()).ToList();

            dbContext.Add(new TestModel { });

            var contextDescriptors = subject.Discover(new TriggerOptions { }, triggerContextTracker, new NullLogger<object>()).ToList();

            Assert.NotEqual(initialContextDescriptors, contextDescriptors);
        }

    }
}
