using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace EntityFrameworkCore.Triggered.Extensions.Tests;

public class TriggerContextExtensionsTests
{
    record TestEntity(int Id);

    class SampleDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(nameof(TriggerContextExtensionsTests));
            optionsBuilder.ConfigureWarnings(warningOptions => {
                warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
            });

        }
    }

    [Fact]
    public void GetDbContext()
    {
        using var expected = new SampleDbContext();
        var entity = new TestEntity(1);
        var triggerContext = new TriggerContext<TestEntity>(expected.Add(entity), null, ChangeType.Added, new Internal.EntityBagStateManager());

        var actual = triggerContext.GetDbContext();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetEntry()
    {
        using var dbContext = new SampleDbContext();
        var entity = new TestEntity(1);
        var expected = dbContext.Add(entity);
        var triggerContext = new TriggerContext<TestEntity>(expected, null, ChangeType.Added, new Internal.EntityBagStateManager());

        var actual = triggerContext.GetEntry();

        Assert.Equal(expected, actual);
    }
}
