using EntityFrameworkCore.Triggered.IntegrationTests.SampleStore.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.IntegrationTests.SampleStore;

public class ApplicationDbContext(string databaseName) : DbContext
{
    readonly string _databaseName = databaseName;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(_databaseName);
        optionsBuilder.UseTriggers(triggerOptions => {
            triggerOptions.AddTrigger<Triggers.Users.SoftDeleteUsers>();
        });
    }

    public DbSet<User> Users { get; set; }
}
