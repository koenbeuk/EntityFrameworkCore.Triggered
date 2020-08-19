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
    public class TriggerContextFactoryTests
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

                optionsBuilder.UseInMemoryDatabase(nameof(TriggerContextFactoryTests));
            }
        }

        [Fact]
        public void Activate_ReturnsInstance()
        {
            var result = TriggerContextFactory<object>.Activate(null, ChangeType.Added);

            Assert.NotNull(result);
        }
    }
}
