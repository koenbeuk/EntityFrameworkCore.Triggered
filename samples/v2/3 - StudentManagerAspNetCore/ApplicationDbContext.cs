using Microsoft.EntityFrameworkCore;

namespace StudentManager
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
                .HasKey(x => new { x.StudentId, x.CourseId });

            modelBuilder.Entity<Audit>()
                .HasKey(x => new { x.Discriminator, x.Id, x.RecordDate });

            modelBuilder.Entity<Course>()
                .HasQueryFilter(x => x.DeletedOn == null);

            modelBuilder.Entity<StudentCourse>()
                .HasQueryFilter(x => x.Course.DeletedOn == null);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<Audit> Audits { get; set; }
    }
}
