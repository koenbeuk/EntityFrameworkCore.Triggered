using System;
using System.Linq;
using EntityFrameworkCore.Triggered;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PerfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int OuterBatches = 10;
            int InnerBatches = 100;

            var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationContext>(options => {
                    options
                        .UseInMemoryDatabase(nameof(ApplicationContext))
                        .UseTriggers();
                })
                .AddSingleton<IBeforeSaveTrigger<Student>, Triggers.SetStudentRegistrationDateTrigger>()
                .AddScoped<IBeforeSaveTrigger<Student>, Triggers.SignStudentUpForMandatoryCourses>()
                .BuildServiceProvider();

            // setup
            {
                using var scope = serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                context.Database.EnsureDeleted();

                context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
                context.SaveChanges();
            }

            // Here we do everything manually
            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
            {
                using var scope = serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
                {
                    var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };
                    context.Students.Add(student);
                }

                context.SaveChanges();
            }

            // validation
            {
                using var scope = serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                var studentCoursesCount = context.StudentCourses.Count();

                if (studentCoursesCount != OuterBatches * InnerBatches)
                {
                    throw new InvalidOperationException();
                }

                Console.WriteLine("Courses: " + studentCoursesCount.ToString());
            }
        }
    }
}
