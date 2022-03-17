using System;
using Microsoft.EntityFrameworkCore;

namespace BlazorTests
{
    public class Count
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Count> Counts { get; set; }

    }
}
