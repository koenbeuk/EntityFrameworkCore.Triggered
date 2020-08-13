using System;
using System.Linq;
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
                .AddDbContext<TriggeredApplicationContext>(options => {
                    options
                        .UseInMemoryDatabase(nameof(TriggeredApplicationContext))
                        .UseTriggers();
                })
                .BuildServiceProvider();

            // setup
            {
                using var scope = serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TriggeredApplicationContext>();

                context.Database.EnsureDeleted();

                context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
                context.SaveChanges();
            }

            // Here we do everything manually
            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
            {
                using var scope = serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TriggeredApplicationContext>();

                for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
                {
                    var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };
                    student.RegistrationDate = DateTimeOffset.Now;

                    context.Students.Add(student);

                    var mandatoryCourses = context.Courses.Where(x => x.IsMandatory).ToList();

                    foreach (var mandatoryCourse in mandatoryCourses)
                    {
                        context.StudentCourses.Add(new StudentCourse {
                            CourseId = mandatoryCourse.Id,
                            StudentId = student.Id
                        });
                    }
                }

                context.SaveChanges();
            }

            // validation
            {
                using var scope = serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TriggeredApplicationContext>();

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
