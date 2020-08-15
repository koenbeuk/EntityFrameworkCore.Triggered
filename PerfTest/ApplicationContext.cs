using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace PerfTest
{

        public class Student
        {
            public Guid Id { get; set; }

            public string DisplayName { get; set; }

            public DateTimeOffset RegistrationDate { get; set; }
        }

        public class Course
        {
            public Guid Id { get; set; }

            public string DisplayName { get; set; }

            public bool IsMandatory { get; set; }

        }

        public class StudentCourse
        {
            public Guid StudentId { get; set; }

            public Guid CourseId { get; set; }

            public Student Student { get; set; }

            public Course Course { get; set; }
        }

        public interface IApplicationContextContract : IDisposable
        {
            DatabaseFacade Database { get; }

            int SaveChanges();

            DbSet<Student> Students { get; set; }

            DbSet<Course> Courses { get; set; }

            DbSet<StudentCourse> StudentCourses { get; set; }
        }

        public class ApplicationContext : TriggeredDbContext, IApplicationContextContract
        {
            public ApplicationContext(DbContextOptions<ApplicationContext> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<StudentCourse>().HasKey(x => new { x.StudentId, x.CourseId });
            }

            public DbSet<Student> Students { get; set; }

            public DbSet<Course> Courses { get; set; }

            public DbSet<StudentCourse> StudentCourses { get; set; }
        }
}
