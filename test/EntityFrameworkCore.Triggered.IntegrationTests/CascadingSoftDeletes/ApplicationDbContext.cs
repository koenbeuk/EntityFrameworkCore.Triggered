using EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes.Models;
using EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes.Triggers;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes
{
    public class ApplicationDbContext(string databaseName) : DbContext
    {
        readonly string _databaseName = databaseName;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
                .UseInMemoryDatabase(_databaseName)
                .UseTriggers(triggerOptions => {
                    triggerOptions.AddTrigger<SoftDelete>();
                });

        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Branch>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

        public DbSet<Branch> Branches => Set<Branch>();
    }


}
