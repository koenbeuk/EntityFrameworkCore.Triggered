using System;
using System.Linq;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
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
                optionsBuilder.EnableServiceProviderCaching(false);
            }
        }


        [Fact]
        public void DiscoverChanges_AddedEntity_CreatesDescriptor()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            dbContext.Entry(new TestModel { }).State = EntityState.Added;

            var triggerContextDescriptor = subject.DiscoverChanges().FirstOrDefault();

            Assert.Equal(ChangeType.Added, triggerContextDescriptor.ChangeType);
        }

        [Fact]
        public void DiscoverChanges_ModifiedEntity_CreatesDescriptor()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            dbContext.Entry(new TestModel { }).State = EntityState.Modified;

            var triggerContextDescriptor = subject.DiscoverChanges().FirstOrDefault();

            Assert.Equal(ChangeType.Modified, triggerContextDescriptor.ChangeType);
        }

        [Fact]
        public void DiscoverChanges_DeletedEntity_CreatesDescriptor()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            dbContext.Entry(new TestModel { }).State = EntityState.Deleted;

            var triggerContextDescriptor = subject.DiscoverChanges().FirstOrDefault();

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

        [Fact]
        public void CaptureDiscoveredChangesAfterAdd_UnchangedEntry_RemovesDiscoveredChange()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            var testModel = new TestModel();
            dbContext.Entry(testModel).State = EntityState.Added;

            var disoveredChanges = subject.DiscoverChanges();
            Assert.Single(disoveredChanges);
            Assert.Single(subject.DiscoveredChanges);

            dbContext.Entry(testModel).State = EntityState.Unchanged;
            disoveredChanges = subject.DiscoverChanges();
            subject.CaptureChanges();
            Assert.Empty(subject.DiscoveredChanges);
        }

        [Fact]
        public void CaptureDiscoveredChangesAfterAdd_DetachedEntry_RemovesDiscoveredChange()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            var testModel = new TestModel();
            dbContext.Entry(testModel).State = EntityState.Added;

            var disoveredChanges = subject.DiscoverChanges();
            Assert.Single(disoveredChanges);
            Assert.Single(subject.DiscoveredChanges);

            dbContext.Entry(testModel).State = EntityState.Detached;
            subject.CaptureChanges();
            Assert.Empty(subject.DiscoveredChanges);
        }

        [Fact]
        public void CaptureDiscoveredChanges_DeletedEntry_UpdatesDiscoveredChange()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            var testModel = new TestModel();
            dbContext.Entry(testModel).State = EntityState.Added;

            var disoveredChanges = subject.DiscoverChanges();
            Assert.Single(disoveredChanges);
            Assert.Single(subject.DiscoveredChanges);

            dbContext.Entry(testModel).State = EntityState.Deleted;
            disoveredChanges = subject.DiscoverChanges();
            Assert.Equal(2, subject.DiscoveredChanges.Count());

            subject.CaptureChanges();
            Assert.Single(subject.DiscoveredChanges);
        }


        [Fact]
        public void UncaptureDiscoveredChanges_OneEntry_RestoresDiscoveredChange()
        {
            using var dbContext = new TestDbContext();
            var subject = new TriggerContextTracker(dbContext.ChangeTracker, new EntityAndTypeRecursionStrategy());

            var testModel = new TestModel();
            dbContext.Entry(testModel).State = EntityState.Added;

            var disoveredChanges = subject.DiscoverChanges();
            dbContext.Entry(testModel).State = EntityState.Unchanged;

            subject.CaptureChanges();
            Assert.Empty(subject.DiscoveredChanges);

            subject.UncaptureChanges();
            Assert.Single(subject.DiscoveredChanges);
        }
    }
}
