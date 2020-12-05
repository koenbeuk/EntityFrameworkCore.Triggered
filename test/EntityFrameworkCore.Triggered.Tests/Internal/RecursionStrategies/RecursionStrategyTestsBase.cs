using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.CascadeStrategies;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal.CascadeStrategies
{
    public abstract class CascadeStrategyTestsBase
    {
        class TestModel { public int Id { get; set; } public int Property1 { get; set; } }

        class TestDbContext : DbContext
        {
            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.EnableServiceProviderCaching(false);
            }
        }

        protected abstract ICascadeStrategy CreateSubject();

        protected abstract bool CanCascadeUnmodifiedExpectedOutcome { get; }
        protected abstract bool CanCascadeModifiedExpectedOutcome { get; }
        protected abstract bool CanCascadeUnmodifiedDifferentTypeExpectedOutcome { get; }
        protected abstract bool CanCascadeModifiedDifferentTypeExpectedOutcome { get; }


        [Fact]
        public void CanCascade_Unmodified()
        {
            using var dbContext = new TestDbContext();
            var subject = CreateSubject();
            var entity = new TestModel { Property1 = 1 };
            dbContext.Add(entity);
            var previousTriggerContextDescriptor = new TriggerContextDescriptor(dbContext.Entry(entity), ChangeType.Added);

            var result = subject.CanCascade(dbContext.Entry(entity), ChangeType.Added, previousTriggerContextDescriptor);

            Assert.Equal(CanCascadeUnmodifiedExpectedOutcome, result);
        }

        [Fact]
        public void CanCascade_Modified()
        {
            using var dbContext = new TestDbContext();
            var subject = CreateSubject();
            var entity = new TestModel { Property1 = 1 };
            dbContext.Add(entity);
            var previousTriggerContextDescriptor = new TriggerContextDescriptor(dbContext.Entry(entity), ChangeType.Added);

            entity.Property1 = 2;
            var result = subject.CanCascade(dbContext.Entry(entity), ChangeType.Added, previousTriggerContextDescriptor);

            Assert.Equal(CanCascadeUnmodifiedExpectedOutcome, result);
        }

        [Fact]
        public void CanCascade_UnmodifiedDifferentType()
        {
            using var dbContext = new TestDbContext();
            var subject = CreateSubject();
            var entity = new TestModel { Property1 = 1 };
            dbContext.Add(entity);
            var previousTriggerContextDescriptor = new TriggerContextDescriptor(dbContext.Entry(entity), ChangeType.Added);

            var result = subject.CanCascade(dbContext.Entry(entity), ChangeType.Modified, previousTriggerContextDescriptor);

            Assert.Equal(CanCascadeUnmodifiedExpectedOutcome, result);
        }

        [Fact]
        public void CanCascade_ModifiedDifferentType()
        {
            using var dbContext = new TestDbContext();
            var subject = CreateSubject();
            var entity = new TestModel { Property1 = 1 };
            dbContext.Add(entity);
            var previousTriggerContextDescriptor = new TriggerContextDescriptor(dbContext.Entry(entity), ChangeType.Added);

            entity.Property1 = 2;
            var result = subject.CanCascade(dbContext.Entry(entity), ChangeType.Modified, previousTriggerContextDescriptor);

            Assert.Equal(CanCascadeUnmodifiedExpectedOutcome, result);
        }
    }
}
