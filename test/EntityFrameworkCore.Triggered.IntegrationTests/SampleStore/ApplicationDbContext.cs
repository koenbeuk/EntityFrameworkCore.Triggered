using EntityFrameworkCore.Triggered.IntegrationTests.SampleStore.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.IntegrationTests.SampleStore
{
    public class ApplicationDbContext
#if EFCORETRIGGERED1
        : TriggeredDbContext
#else
        : DbContext
#endif
    {
        readonly string _databaseName;

        public ApplicationDbContext(string databaseName)
        {
            _databaseName = databaseName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(_databaseName);
            optionsBuilder.UseTriggers(triggerOptions => {
                triggerOptions.AddTrigger<Triggers.Users.SoftDeleteUsers>();
            });
        }

        public DbSet<User> Users { get; set; }
    }
}
