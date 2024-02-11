using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Benchmarks
{
    [MemoryDiagnoser]
    public class PlainOverheadBenchmarks
    {
        IServiceProvider _serviceProvider;

        [GlobalSetup]
        public void GlobalSetup() => _serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationContext>(options => {
                    options
                        .UseInMemoryDatabase(nameof(WithDbContext));
                })
                .AddDbContext<TriggeredApplicationContext>(options => {
                    options
                        .UseInMemoryDatabase(nameof(WithTriggeredDbContext))
                        .UseTriggers();
                })
                .BuildServiceProvider();

        [Params(50)]
        public int OuterBatches;

        [Params(1, 10, 100)]
        public int InnerBatches;

        void Execute<TApplicationContext>()
            where TApplicationContext : IApplicationContextContract
        {
            // setup
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TApplicationContext>() as IApplicationContextContract;

                context.Database.EnsureDeleted();

                context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
                context.SaveChanges();
            }

            // Here we do everything manually
            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TApplicationContext>();

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
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TApplicationContext>();

                var studentCoursesCount = context.StudentCourses.Count();

                if (studentCoursesCount != OuterBatches * InnerBatches)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        [Benchmark(Baseline = true)]
        public void WithDbContext() => Execute<ApplicationContext>();

        [Benchmark]
        public void WithTriggeredDbContext() => Execute<TriggeredApplicationContext>();
    }
}
