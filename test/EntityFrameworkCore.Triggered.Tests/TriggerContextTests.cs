using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerContextTests
    {
        class TestModel { public int Id { get; set; } }

        class TestDbContext : DbContext
        {
            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("test");
            }
        }

        [Fact]
        public void UnmodifiedEntity_WhenTypeAdded_IsEmpty()
        {
            using var dbContext = new TestDbContext();
            var sample1 = new TestModel() { Id = 1 };
            var subject = new TriggerContext<object>(dbContext.Entry(sample1), ChangeType.Added);

            Assert.Null(subject.UnmodifiedEntity);
        }

        [Fact]
        public void UnmodifiedEntity_WhenTypeDeleted_IsNotEmpty()
        {
            using var dbContext = new TestDbContext();
            var sample1 = new TestModel();
            var subject = new TriggerContext<object>(dbContext.Entry(sample1), ChangeType.Deleted);

            Assert.NotNull(subject.UnmodifiedEntity);
        }

        [Fact]
        public void UnmodifiedEntity_WhenTypeModified_IsNotEmpty()
        {
            using var dbContext = new TestDbContext();
            var sample1 = new TestModel();
            var subject = new TriggerContext<object>(dbContext.Entry(sample1), ChangeType.Modified);

            Assert.NotNull(subject.UnmodifiedEntity);
        }

        [Fact]
        public void Entity_IsNeverEmpty()
        {
            using var dbContext = new TestDbContext();
            var sample1 = new TestModel();
            var subject = new TriggerContext<object>(dbContext.Entry(sample1), default);

            Assert.NotNull(subject.Entity);
        }

        [Fact]
        public void Type_IsNotEmpty()
        {
            using var dbContext = new TestDbContext();
            var sample1 = new TestModel();
            var subject = new TriggerContext<object>(dbContext.Entry(sample1), ChangeType.Modified);

            Assert.Equal(ChangeType.Modified, subject.ChangeType);
        }
    }
}
