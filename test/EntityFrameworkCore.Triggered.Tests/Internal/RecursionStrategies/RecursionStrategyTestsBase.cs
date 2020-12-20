using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal.RecursionStrategies
{
    public abstract class RecursionStrategyTestsBase
    {
        class TestModel { public int Id { get; set; } public int Property1 { get; set; } }

        class TestDbContext : DbContext
        {
            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.ConfigureWarnings(warningOptions => {
                    warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                });
            }
        }

        protected abstract IRecursionStrategy CreateSubject();

        protected abstract bool CanRecurseUnmodifiedExpectedOutcome { get; }
        protected abstract bool CanRecurseModifiedExpectedOutcome { get; }
        protected abstract bool CanRecurseUnmodifiedDifferentTypeExpectedOutcome { get; }
        protected abstract bool CanRecurseModifiedDifferentTypeExpectedOutcome { get; }


        [Fact]
        public void CanRecurse_Unmodified()
        {
            using var dbContext = new TestDbContext();
            var subject = CreateSubject();
            var entity = new TestModel { Property1 = 1 };
            dbContext.Add(entity);
            var previousTriggerContextDescriptor = new TriggerContextDescriptor(dbContext.Entry(entity), ChangeType.Added);

            var result = subject.CanRecurse(dbContext.Entry(entity), ChangeType.Added, previousTriggerContextDescriptor);

            Assert.Equal(CanRecurseUnmodifiedExpectedOutcome, result);
        }

        [Fact]
        public void CanRecurse_Modified()
        {
            using var dbContext = new TestDbContext();
            var subject = CreateSubject();
            var entity = new TestModel { Property1 = 1 };
            dbContext.Add(entity);
            var previousTriggerContextDescriptor = new TriggerContextDescriptor(dbContext.Entry(entity), ChangeType.Added);

            entity.Property1 = 2;
            var result = subject.CanRecurse(dbContext.Entry(entity), ChangeType.Added, previousTriggerContextDescriptor);

            Assert.Equal(CanRecurseUnmodifiedExpectedOutcome, result);
        }

        [Fact]
        public void CanRecurse_UnmodifiedDifferentType()
        {
            using var dbContext = new TestDbContext();
            var subject = CreateSubject();
            var entity = new TestModel { Property1 = 1 };
            dbContext.Add(entity);
            var previousTriggerContextDescriptor = new TriggerContextDescriptor(dbContext.Entry(entity), ChangeType.Added);

            var result = subject.CanRecurse(dbContext.Entry(entity), ChangeType.Modified, previousTriggerContextDescriptor);

            Assert.Equal(CanRecurseUnmodifiedExpectedOutcome, result);
        }

        [Fact]
        public void CanRecurse_ModifiedDifferentType()
        {
            using var dbContext = new TestDbContext();
            var subject = CreateSubject();
            var entity = new TestModel { Property1 = 1 };
            dbContext.Add(entity);
            var previousTriggerContextDescriptor = new TriggerContextDescriptor(dbContext.Entry(entity), ChangeType.Added);

            entity.Property1 = 2;
            var result = subject.CanRecurse(dbContext.Entry(entity), ChangeType.Modified, previousTriggerContextDescriptor);

            Assert.Equal(CanRecurseUnmodifiedExpectedOutcome, result);
        }
    }
}
