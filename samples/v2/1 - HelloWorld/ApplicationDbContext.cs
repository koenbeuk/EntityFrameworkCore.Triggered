using System;
using Microsoft.EntityFrameworkCore;

namespace PrimarySchool
{
    public class Student
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public DateTime RegistrationDate { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseInMemoryDatabase("HelloWorld")
                .UseTriggers(triggerOptions => {
                    triggerOptions.AddTrigger<Triggers.StudentAssignRegistrationDate>();
                });

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Student> Students { get; set; }
    }
}
