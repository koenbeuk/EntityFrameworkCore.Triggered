using Microsoft.EntityFrameworkCore;

namespace BlazorTests;

public class Count
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Count> Counts { get; set; }

}
