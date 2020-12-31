using System;
using System.Linq;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using EntityFrameworkCore.Triggers;
using Microsoft.Diagnostics.Runtime.Interop;
using Microsoft.Diagnostics.Tracing.Analysis.GC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Benchmarks
{
    [MemoryDiagnoser]
    public class EmbracingFeaturesBenchmarks
    {
        private IServiceProvider _serviceProvider;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationContext>(options => {
                    options
                        .UseInMemoryDatabase(nameof(WithDbContext));
                })
                .AddTriggeredDbContext<TriggeredApplicationContext>(options => {
                    options
                        .UseInMemoryDatabase(nameof(WithTriggeredDbContext))
                        .UseTriggers();
                })
                .AddDbContext<ApplicationContextWithTriggers>(options => {
                    options
                        .UseInMemoryDatabase(nameof(WithDbContextWithTriggers));
                })
                .AddDbContext<RamsesApplicationContext>(options => {
                    options
                        .UseInMemoryDatabase(nameof(RamsesApplicationContext));
                })
                .AddSingleton<IBeforeSaveTrigger<Student>, Triggers.SetStudentRegistrationDateTrigger>()
                .AddScoped<IBeforeSaveTrigger<Student>, Triggers.SignStudentUpForMandatoryCourses>()
                .AddTriggers()
                .AddSingleton(typeof(ITriggers<,>), typeof(Triggers<,>))
                .AddSingleton(typeof(ITriggers<>), typeof(Triggers<>))
                .AddSingleton(typeof(ITriggers), typeof(EntityFrameworkCore.Triggers.Triggers))
                .BuildServiceProvider();

            Triggers<Student, ApplicationContextWithTriggers>.GlobalInserting.Add(entry => {
                entry.Entity.RegistrationDate = DateTimeOffset.Now;
            });

            Triggers<Student, ApplicationContextWithTriggers>.GlobalInserting.Add(entry => {
                var mandatoryCourses = entry.Context.Courses.Where(x => x.IsMandatory).ToList();

                foreach (var mandatoryCourse in mandatoryCourses)
                {
                    entry.Context.StudentCourses.Add(new StudentCourse {
                        CourseId = mandatoryCourse.Id,
                        StudentId = entry.Entity.Id
                    });
                }
            });
        }

        [Params(50)]
        public int OuterBatches;

        [Params(1, 10, 100)]
        public int InnerBatches;

        private void Validate(IApplicationContextContract applicationContextContract)
        {
            var studentCoursesCount = applicationContextContract.StudentCourses.Count();
            var expectedCoursesCount = OuterBatches * InnerBatches;

            if (studentCoursesCount != expectedCoursesCount)
            {
                throw new InvalidOperationException($"Found {studentCoursesCount}, expected {expectedCoursesCount}");
            }
        }

        [Benchmark(Baseline = true)]
        public void WithDbContext()
        {
            // setup
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                context.Database.EnsureDeleted();

                context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
                context.SaveChanges();
            }

            // execute
            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
            {
                using var scope = _serviceProvider.CreateScope();
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

            // validate
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                Validate(context);
            }
        }

        [Benchmark]
        public void WithDbContextWithTriggers()
        {
            // setup
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContextWithTriggers>();

                context.Database.EnsureDeleted();

                context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
                context.SaveChanges();
            }

            // execute
            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContextWithTriggers>();

                for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
                {
                    var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };
                    context.Add(student);
                }

                context.SaveChanges();
            }

            // validate
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContextWithTriggers>();

                Validate(context);
            }
        }

        [Benchmark]
        public void WithTriggeredDbContext()
        {
            // setup
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TriggeredApplicationContext>();

                context.Database.EnsureDeleted();

                context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
                context.SaveChanges();
            }

            // execute
            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TriggeredApplicationContext>();

                for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
                {
                    var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };
                    context.Add(student);
                }

                context.SaveChanges();
            }


            // validate
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<TriggeredApplicationContext>();

                Validate(context);
            }
        }


        [Benchmark]
        public void WithRamsesDbContext()
        {
            // setup
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<RamsesApplicationContext>();

                context.Database.EnsureDeleted();

                context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
                context.SaveChanges();
            }

            // execute
            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<RamsesApplicationContext>();

                for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
                {
                    var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };

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

            // validate
            {
                using var scope = _serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<RamsesApplicationContext>();

                Validate(context);
            }
        }
    }
}
