using System;
using System.Linq;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Runtime.Interop;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Benchmarks
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private IServiceProvider _serviceProvider;

        [ThreadStatic] // Used by triggers to gain access to the the external scoped service provider (mimicks ASP.NET core integration)
        private IServiceProvider _scopedServiceProvider;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationContext>(options => {
                    options
                        .UseInMemoryDatabase(nameof(WithoutTriggers))
                        .EnableSensitiveDataLogging();
                })
                .AddDbContext<TriggeredApplicationContext>(options => {
                    options
                        .UseInMemoryDatabase(nameof(WithTriggers))
                        .EnableSensitiveDataLogging()
                        .UseTriggers(triggerOptions => {
                            triggerOptions.UseApplicationScopedServiceProviderAccessor(_ => _scopedServiceProvider);
                        });
                })
                .AddSingleton<IBeforeSaveTrigger<Student>, Triggers.SetStudentRegistrationDateTrigger>()
                .AddScoped<IBeforeSaveTrigger<Student>, Triggers.SignStudentUpForMandatoryCourses>()
                .BuildServiceProvider();
        }

        [Params(100)]
        public int OuterBatches;

        [Params(1, 10, 100)]
        public int InnerBatches;

        [IterationSetup]
        public void IterationSetup()
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            
            context.Database.EnsureDeleted();

            var course = context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
            context.SaveChanges();
        }

        [Benchmark(Baseline = true)]
        public void WithoutTriggers()
        {
            // Here we do everything manually
            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
            {
                using var scope = _serviceProvider.CreateScope();
                _scopedServiceProvider = scope.ServiceProvider;
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
                {
                    var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };
                    student.RegistrationDate = DateTimeOffset.Now;

                    context.Add(student);

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
        }

        [Benchmark]
        public void WithTriggers()
        {
            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
            {
                using var scope = _serviceProvider.CreateScope();
                _scopedServiceProvider = scope.ServiceProvider;
                using var context = scope.ServiceProvider.GetRequiredService<TriggeredApplicationContext>();

                for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
                {
                    var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };
                    student.RegistrationDate = DateTimeOffset.Now;

                    context.Add(student);
                }

                context.SaveChanges();
            }
        }
    }
}
