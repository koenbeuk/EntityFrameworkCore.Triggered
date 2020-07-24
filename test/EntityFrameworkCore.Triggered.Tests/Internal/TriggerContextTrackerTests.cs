using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerContextTrackerTests
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
            }
        }


        [Fact]
        public void DiscoverChanges_AddedEntity_CreatesDescriptor()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            dbContext.Entry(new TestModel { }).State = EntityState.Added;

            var triggerContextDescriptor = subject.DiscoverChanges().FirstOrDefault();

            Assert.NotNull(triggerContextDescriptor);
            Assert.Equal(ChangeType.Added, triggerContextDescriptor.ChangeType);
        }

        [Fact]
        public void DiscoverChanges_ModifiedEntity_CreatesDescriptor()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            dbContext.Entry(new TestModel { }).State = EntityState.Modified;

            var triggerContextDescriptor = subject.DiscoverChanges().FirstOrDefault();

            Assert.NotNull(triggerContextDescriptor);
            Assert.Equal(ChangeType.Modified, triggerContextDescriptor.ChangeType);
        }

        [Fact]
        public void DiscoverChanges_DeletedEntity_CreatesDescriptor()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            dbContext.Entry(new TestModel { }).State = EntityState.Deleted;

            var triggerContextDescriptor = subject.DiscoverChanges().FirstOrDefault();

            Assert.NotNull(triggerContextDescriptor);
            Assert.Equal(ChangeType.Deleted, triggerContextDescriptor.ChangeType);
        }

        [Fact]
        public void DiscoveredChanges_NoCallToDiscoverChanges_ReturnsNull()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            var discoveredChanges = subject.DiscoveredChanges;

            Assert.Null(discoveredChanges);
        }

        [Fact]
        public void DiscoveredChanges_MultipleCallsToDiscoverChanges_ReturnsAllChanges()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            dbContext.Add(new TestModel { });
            subject.DiscoverChanges().Count();

            dbContext.Add(new TestModel { });
            subject.DiscoverChanges().Count();

            var discoveredChanges = subject.DiscoveredChanges;

            Assert.Equal(2, discoveredChanges.Count());
        }
    }
}
