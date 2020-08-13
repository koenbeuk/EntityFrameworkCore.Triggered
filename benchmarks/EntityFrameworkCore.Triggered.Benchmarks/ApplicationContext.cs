using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered.Benchmarks
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

    public class ApplicationContext : DbContext, IApplicationContextContract
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
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

    public class ApplicationContextWithTriggers : DbContextWithTriggers, IApplicationContextContract
    {
        public ApplicationContextWithTriggers(DbContextOptions<ApplicationContextWithTriggers> options) : base(options)
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

    public class TriggeredApplicationContext : TriggeredDbContext, IApplicationContextContract
    {
        public TriggeredApplicationContext(DbContextOptions<TriggeredApplicationContext> options) : base(options)
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
