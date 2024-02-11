using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal;

public class TriggerContextTests
{
    class TestModel { public int Id { get; set; } public string Name { get; set; } }

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

    [Fact]
    public void UnmodifiedEntity_WhenTypeAdded_IsEmpty()
    {
        using var dbContext = new TestDbContext();
        var sample1 = new TestModel() { Id = 1 };
        var subject = new TriggerContext<object>(dbContext.Entry(sample1), dbContext.Entry(sample1).OriginalValues, ChangeType.Added, new());

        Assert.Null(subject.UnmodifiedEntity);
    }

    [Fact]
    public void UnmodifiedEntity_WhenTypeDeleted_IsNotEmpty()
    {
        using var dbContext = new TestDbContext();
        var sample1 = new TestModel();
        var subject = new TriggerContext<object>(dbContext.Entry(sample1), dbContext.Entry(sample1).OriginalValues, ChangeType.Deleted, new());

        Assert.NotNull(subject.UnmodifiedEntity);
    }

    [Fact]
    public void UnmodifiedEntity_WhenTypeModified_IsNotEmpty()
    {
        using var dbContext = new TestDbContext();
        var sample1 = new TestModel();
        var subject = new TriggerContext<object>(dbContext.Entry(sample1), dbContext.Entry(sample1).OriginalValues, ChangeType.Modified, new());

        Assert.NotNull(subject.UnmodifiedEntity);
    }

    [Fact]
    public void UnmodifiedEntity_WhenTypeModified_HoldsUnmodifiedStateBeforeSaveChanges()
    {
        using var dbContext = new TestDbContext();
        var sample1 = new TestModel { Name = "test1" };
        dbContext.Add(sample1);
        dbContext.SaveChanges();

        var subject = new TriggerContext<TestModel>(dbContext.Entry(sample1), dbContext.Entry(sample1).OriginalValues, ChangeType.Modified, new());
        sample1.Name = "test2";

        Assert.NotNull(subject.UnmodifiedEntity);
        Assert.Equal("test1", subject.UnmodifiedEntity.Name);

        dbContext.SaveChanges();
    }

    [Fact]
    public void UnmodifiedEntity_WhenTypeModified_HoldsUnmodifiedStateAfterSaveChanges()
    {
        using var dbContext = new TestDbContext();
        var sample1 = new TestModel { Name = "test1" };
        dbContext.Add(sample1);
        dbContext.SaveChanges();

        var subject = new TriggerContext<TestModel>(dbContext.Entry(sample1), dbContext.Entry(sample1).OriginalValues.Clone(), ChangeType.Modified, new());
        sample1.Name = "test2";

        dbContext.SaveChanges();

        Assert.NotNull(subject.UnmodifiedEntity);
        Assert.Equal("test1", subject.UnmodifiedEntity.Name);
    }

    [Fact]
    public void Entity_IsNeverEmpty()
    {
        using var dbContext = new TestDbContext();
        var sample1 = new TestModel();
        var subject = new TriggerContext<object>(dbContext.Entry(sample1), dbContext.Entry(sample1).OriginalValues, default, new());

        Assert.NotNull(subject.Entity);
    }

    [Fact]
    public void Type_IsNotEmpty()
    {
        using var dbContext = new TestDbContext();
        var sample1 = new TestModel();
        var subject = new TriggerContext<object>(dbContext.Entry(sample1), dbContext.Entry(sample1).OriginalValues, ChangeType.Modified, new());

        Assert.Equal(ChangeType.Modified, subject.ChangeType);
    }

    [Fact]
    public void EntityBag_ConsistentlyReturnsTheSameInstance()
    {
        using var dbContext = new TestDbContext();
        var sample1 = new TestModel();
        var subject = new TriggerContext<object>(dbContext.Entry(sample1), dbContext.Entry(sample1).OriginalValues, ChangeType.Modified, new());

        var expectedInstance = subject.Items;
        Assert.Equal(expectedInstance, subject.Items);
    }
}
