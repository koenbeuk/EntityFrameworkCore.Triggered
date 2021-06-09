using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.IntegrationTests.LifetimeTests
{
    public class ApplicationDbContext
#if EFCORETRIGGERED1
        : TriggeredDbContext
#else
        : DbContext
#endif
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
