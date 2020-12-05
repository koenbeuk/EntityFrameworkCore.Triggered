using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal.CascadeStrategies;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests
{
    public class TriggerServiceTests
    {
        public class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext()
            {
            }

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.EnableServiceProviderCaching(false);
                optionsBuilder.UseInMemoryDatabase("test");
            }
        }

        [Fact]
        public void Current_WithoutSession_ReturnsNull()
        {
            var subject = new TriggerService(new TriggerDiscoveryServiceStub(), new NoCascadeStrategy(), new LoggerFactory(), new OptionsSnapshotStub<TriggerOptions>());
            Assert.Null(subject.Current);
        }

        [Fact]
        public void Current_WithSingleSession_ReturnsSession()
        {
            var subject = new TriggerService(new TriggerDiscoveryServiceStub(), new NoCascadeStrategy(), new LoggerFactory(), new OptionsSnapshotStub<TriggerOptions>());
            var dbContext = new TestDbContext();

            var triggerSession = subject.CreateSession(dbContext, null);

            Assert.Equal(subject.Current, triggerSession);
        }

        [Fact]
        public void Current_WithMultipleSessions_ReturnsLatestSession()
        {
            var subject = new TriggerService(new TriggerDiscoveryServiceStub(), new NoCascadeStrategy(), new LoggerFactory(), new OptionsSnapshotStub<TriggerOptions>());
            var dbContext = new TestDbContext();

            subject.CreateSession(dbContext, null);
            var triggerSession = subject.CreateSession(dbContext, null);

            Assert.Equal(subject.Current, triggerSession);
        }
    }
}
