using System;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PrimarySchool
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceScope serviceScope = null;

            var serviceProvider = new ServiceCollection()
               .AddDbContext<ApplicationContext>(options => {
                   options
                      .UseSqlite("Data source=test.db")
                      .UseTriggers(triggerOptions => {
                          triggerOptions.UseApplicationScopedServiceProviderAccessor(_ => serviceScope.ServiceProvider);
                      });
               })
               .AddScoped<IBeforeSaveTrigger<Student>, Triggers.StudentSignupToMandatoryCourses>()
               .BuildServiceProvider();

            serviceScope = serviceProvider.CreateScope();

            var applicationContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();

            applicationContext.Database.EnsureDeleted();
            applicationContext.Database.EnsureCreated();

            applicationContext.Courses.Add(new Course {
                Id = 1,
                DisplayName = "Mathematics",
                IsMandatory = true
            });

            applicationContext.Courses.Add(new Course {
                Id = 2,
                DisplayName = "Art",
                IsMandatory = false
            });

            applicationContext.SaveChanges();

            applicationContext.Students.Add(new Student {
                Id = 1,
                DisplayName = "Joe"
            });

            applicationContext.SaveChanges();

            var joesCourses = applicationContext.StudentCourses
                .Where(x => x.StudentId == 1)
                .Select(x => x.Course.DisplayName);

            foreach (var course in joesCourses)
            {
                Console.WriteLine($"Joe is registered for course {course}");
            }
        }
    }
}
