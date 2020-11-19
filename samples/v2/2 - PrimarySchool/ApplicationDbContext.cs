using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace PrimarySchool
{
    public class Student
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public bool IsMandatory { get; set; }

    }

    public class StudentCourse
    {
        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public Student Student { get; set; }

        public Course Course { get; set; }
    }


    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>()
                .HasKey(x => new { x.StudentId, x.CourseId });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }
    }
}
