using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class RecursiveTriggerContextDiscoveryStrategyTests
    {
        class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        class TestDbContext : DbContext
        {
            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseTriggers(triggerOptions => {
                });
            }
        }


        [Fact]
        public void DiscoverChanges_MultipleCalls_ReturnsDeltaOfChanges()
        {
            using var dbContext = new TestDbContext();
            var subject = new RecursiveTriggerContextDiscoveryStrategy("test");
            var triggerContextTracker = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());
            triggerContextTracker.DiscoverChanges().Count();
            var initialContextDescriptors = subject.Discover(new TriggerOptions { }, triggerContextTracker, new NullLogger<object>()).ToList();

            dbContext.Add(new TestModel { });

            var contextDescriptors = subject.Discover(new TriggerOptions { }, triggerContextTracker, new NullLogger<object>()).ToList();

            Assert.NotEqual(initialContextDescriptors, contextDescriptors);
        }
    }
}
