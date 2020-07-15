using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Internal;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests.Internal
{
    public class ChangeEventTests
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
            var subject = new ChangeEvent<object>(ChangeType.Added, dbContext.Entry(sample1));

            Assert.Null(subject.UnmodifiedEntity);
        }

        [Fact]
        public void UnmodifiedEntity_WhenTypeDeleted_IsNotEmpty()
        {
            using var dbContext = new TestDbContext();
            var sample1 = new TestModel();
            var subject = new ChangeEvent<object>(ChangeType.Deleted, dbContext.Entry(sample1));

            Assert.NotNull(subject.UnmodifiedEntity);
        }

        [Fact]
        public void UnmodifiedEntity_WhenTypeModified_IsNotEmpty()
        {
            using var dbContext = new TestDbContext();
            var sample1 = new TestModel();
            var subject = new ChangeEvent<object>(ChangeType.Modified, dbContext.Entry(sample1));

            Assert.NotNull(subject.UnmodifiedEntity);
        }

        [Fact]
        public void Entity_IsNeverEmpty()
        {
            using var dbContext = new TestDbContext();
            var sample1 = new TestModel();
            var subject = new ChangeEvent<object>(default, dbContext.Entry(sample1));

            Assert.NotNull(subject.Entity);
        }

        [Fact]
        public void Type_IsNotEmpty()
        {
            using var dbContext = new TestDbContext();
            var sample1 = new TestModel();
            var subject = new ChangeEvent<object>(ChangeType.Modified, dbContext.Entry(sample1));

            Assert.Equal(ChangeType.Modified, subject.Type);
        }
    }
}
