using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.IntegrationTests.EntityBags
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
