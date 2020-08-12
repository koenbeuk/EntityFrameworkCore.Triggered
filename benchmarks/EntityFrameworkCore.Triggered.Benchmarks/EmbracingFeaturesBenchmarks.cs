//using System;
//using System.Linq;
//using BenchmarkDotNet;
//using BenchmarkDotNet.Attributes;
//using EntityFrameworkCore.Triggers;
//using Microsoft.Diagnostics.Runtime.Interop;
//using Microsoft.Diagnostics.Tracing.Analysis.GC;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;

//namespace EntityFrameworkCore.Triggered.Benchmarks
//{
//    [MemoryDiagnoser]
//    public class EmbracingFeaturesBenchmarks
//    {
//        private IServiceProvider _serviceProvider;

//        [GlobalSetup]
//        public void GlobalSetup()
//        {
//            _serviceProvider = new ServiceCollection()
//                .AddDbContext<ApplicationContext>(options => { 
//                    options
//                        .UseInMemoryDatabase(nameof(WithDbContext));
//                })
//                .AddDbContext<TriggeredApplicationContext>(options => {
//                    options
//                        .UseInMemoryDatabase(nameof(WithTriggeredDbContext))
//                        .UseTriggers(triggerOptions => {
//                            triggerOptions.UseApplicationScopedServiceProviderAccessor(_ => _scopedServiceProvider);
//                        });
//                })
//                .AddSingleton<IBeforeSaveTrigger<Student>, Triggers.SetStudentRegistrationDateTrigger>()
//                .AddScoped<IBeforeSaveTrigger<Student>, Triggers.SignStudentUpForMandatoryCourses>()
//                .AddDbContext<ApplicationContextWithTriggers>(options => {
//                    options
//                        .UseInMemoryDatabase(nameof(WithDbContextWithTriggers));
//                })
//                .AddTriggers()
//                .AddSingleton(typeof(ITriggers<,>), typeof(Triggers<,>))
//                .AddSingleton(typeof(ITriggers<>), typeof(Triggers<>))
//                .AddSingleton(typeof(ITriggers), typeof(EntityFrameworkCore.Triggers.Triggers))
//                .BuildServiceProvider();
//        }

//        [Params(50)]
//        public int OuterBatches;

//        [Params(1, 10, 100)]
//        public int InnerBatches;

//        [Benchmark(Baseline = true)]
//        public void WithDbContext()
//        {
//            // setup
//            {
//                using var scope = _serviceProvider.CreateScope();
//                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

//                context.Database.EnsureDeleted();

//                context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
//                context.SaveChanges();
//            }

//            // Here we do everything manually
//            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
//            {
//                using var scope = _serviceProvider.CreateScope();
//                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

//                for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
//                {
//                    var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };
//                    student.RegistrationDate = DateTimeOffset.Now;

//                    context.Add(student);

//                    var mandatoryCourses = context.Courses.Where(x => x.IsMandatory).ToList();

//                    foreach (var mandatoryCourse in mandatoryCourses)
//                    {
//                        context.StudentCourses.Add(new StudentCourse {
//                            CourseId = mandatoryCourse.Id,
//                            StudentId = student.Id
//                        });
//                    }
//                }
                
//                context.SaveChanges();
//            }

//            {
//                using var scope = _serviceProvider.CreateScope();
//                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

//                var studentCoursesCount = context.StudentCourses.Count();
                
//                if (studentCoursesCount != OuterBatches * InnerBatches)
//                {
//                    throw new InvalidOperationException();
//                }
//            }
//        }

//        [Benchmark]
//        public void WithDbContextWithTriggers()
//        {
//            // setup
//            {
//                using var scope = _serviceProvider.CreateScope();
//                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContextWithTriggers>();

//                context.Database.EnsureDeleted();

//                context.Courses.Add(new Course { Id = Guid.NewGuid(), DisplayName = "Test", IsMandatory = true });
//                context.SaveChanges();
//            }

//            // Here we do everything manually
//            for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
//            {
//                using var scope = _serviceProvider.CreateScope();
//                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContextWithTriggers>();

//                for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
//                {
//                    var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };
//                    student.RegistrationDate = DateTimeOffset.Now;

//                    context.Add(student);

//                    var mandatoryCourses = context.Courses.Where(x => x.IsMandatory).ToList();

//                    foreach (var mandatoryCourse in mandatoryCourses)
//                    {
//                        context.StudentCourses.Add(new StudentCourse {
//                            CourseId = mandatoryCourse.Id,
//                            StudentId = student.Id
//                        });
//                    }
//                }

//                context.SaveChanges();
//            }

//            {
//                using var scope = _serviceProvider.CreateScope();
//                using var context = scope.ServiceProvider.GetRequiredService<ApplicationContextWithTriggers>();

//                var studentCoursesCount = context.StudentCourses.Count();

//                if (studentCoursesCount != OuterBatches * InnerBatches)
//                {
//                    throw new InvalidOperationException($"Expected {OuterBatches * InnerBatches}, got: {studentCoursesCount}");
//                }
//            }
//        }

//        //[Benchmark]
//        //public void WithTriggeredDbContext()
//        //{
//        //    for (var outerBatch = 0; outerBatch < OuterBatches; outerBatch++)
//        //    {
//        //        using var scope = _serviceProvider.CreateScope();
//        //        _scopedServiceProvider = scope.ServiceProvider;
//        //        using var context = scope.ServiceProvider.GetRequiredService<TriggeredApplicationContext>();

//        //        for (var innerBatch = 0; innerBatch < InnerBatches; innerBatch++)
//        //        {
//        //            var student = new Student { Id = Guid.NewGuid(), DisplayName = "Test" };
//        //            student.RegistrationDate = DateTimeOffset.Now;

//        //            context.Add(student);
//        //        }

//        //        context.SaveChanges();
//        //    }
//        //}
//    }
//}
