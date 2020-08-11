using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests
{
    public class TriggerSessionTests
    {
        public class TestModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions options) : base(options)
            {
            }

            public TestDbContext()
            {
            }

            public TriggerStub<TestModel> TriggerStub { get; } = new TriggerStub<TestModel>();

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.EnableServiceProviderCaching(false);
                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseTriggers(triggerOptions => {
                    triggerOptions.AddTrigger(TriggerStub);
                });
            }
        }

        protected static ITriggerSession CreateSubject(DbContext context)
            => context.Database.GetService<ITriggerService>().CreateSession(context);

        [Fact]
        public async Task RaiseBeforeSaveTriggers_RaisesNothingOnNoChanges()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            await subject.RaiseBeforeSaveTriggers();

            Assert.Empty(context.TriggerStub.BeforeSaveInvocations);
        }

        [Fact]
        public async Task RaiseAfterSaveTriggers_RaisesNothingOnNoChanges()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            subject.DiscoverChanges();
            await subject.RaiseAfterSaveTriggers();

            Assert.Empty(context.TriggerStub.AfterSaveInvocations);
        }

        [Fact]
        public async Task RaiseBeforeSaveTriggers_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = 1,
                Name = "test1"
            });

            await subject.RaiseBeforeSaveTriggers();

            Assert.Equal(1, context.TriggerStub.BeforeSaveInvocations.Count);
        }

        [Fact]
        public async Task RaiseAfterSaveTriggers_WithoutCallToRaiseBeforeSaveTriggers_Throws()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = 1,
                Name = "test1"
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => subject.RaiseAfterSaveTriggers());
        }

        [Fact]
        public async Task RaiseAfterSaveTriggers_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = 1,
                Name = "test1"
            });

            subject.DiscoverChanges();
            await subject.RaiseAfterSaveTriggers();

            Assert.Equal(1, context.TriggerStub.AfterSaveInvocations.Count);
        }

        [Fact]
        public async Task RaiseBeforeSaveTriggers_ThrowsOnCancelledException()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = 1,
                Name = "test1"
            });

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await subject.RaiseBeforeSaveTriggers(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task RaiseAfterSaveTriggers_ThrowsOnCancelledException()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = 1,
                Name = "test1"
            });

            subject.DiscoverChanges();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await subject.RaiseAfterSaveTriggers(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task RaiseBeforeSaveTriggers_RecursiveCall_SkipsDiscoveredChanges()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TriggerStub.BeforeSaveHandler = (_1, _2) => {
                if (context.TriggerStub.BeforeSaveInvocations.Count > 1)
                {
                    return Task.CompletedTask;
                }
                return subject.RaiseBeforeSaveTriggers(default);
            };

            context.TestModels.Add(new TestModel {
                Id = 1,
                Name = "test1"
            });

            subject.DiscoverChanges();
            await subject.RaiseBeforeSaveTriggers();

            Assert.NotEmpty(context.TriggerStub.BeforeSaveInvocations);
        }

        [Fact]
        public async Task RaiseBeforeSaveTriggers_SkipDetectedChangesAsTrue_ExcludesDetectedChanges()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = 1,
                Name = "test1"
            });

            subject.DiscoverChanges();
            await subject.RaiseBeforeSaveTriggers(true);

            Assert.Empty(context.TriggerStub.BeforeSaveInvocations);
        }

        [Fact]
        public async Task RaiseBeforeSaveTriggers_SkipDetectedChangesAsFalse_IncludesDetectedChanges()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = 1,
                Name = "test1"
            });

            subject.DiscoverChanges();
            await subject.RaiseBeforeSaveTriggers();

            Assert.NotEmpty(context.TriggerStub.BeforeSaveInvocations);
        }

        [Fact]
        public void RaiseBeforeSaveTriggers_MultipleEntities_SortByPriorities()
        {
            var capturedInvocations = new List<(string, ITriggerContext<TestModel>)>();

            var earlyTrigger = new TriggerStub<TestModel> {
                Priority = CommonTriggerPriority.Early,
                BeforeSaveHandler = (context, _) => {
                    capturedInvocations.Add(("Early", context));
                    return Task.CompletedTask;
                }
            };

            var lateTrigger = new TriggerStub<TestModel> {
                Priority = CommonTriggerPriority.Late,
                BeforeSaveHandler = (context, _) => {
                    capturedInvocations.Add(("Late", context));
                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IBeforeSaveTrigger<TestModel>>(lateTrigger)
                .AddSingleton<IBeforeSaveTrigger<TestModel>>(earlyTrigger)
                .AddTriggeredDbContext<TestDbContext>(options => {
                    options.UseInMemoryDatabase("Test");
                })
                .BuildServiceProvider();

            var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var subject = CreateSubject(dbContext);

            dbContext.TestModels.Add(new TestModel { Id = 1 });
            dbContext.TestModels.Add(new TestModel { Id = 2 });

            subject.RaiseBeforeSaveTriggers();

            Assert.Equal(4, capturedInvocations.Count);
            Assert.Equal("Early", capturedInvocations[0].Item1);
            Assert.Equal("Early", capturedInvocations[1].Item1);
            Assert.Equal("Late", capturedInvocations[2].Item1);
            Assert.Equal("Late", capturedInvocations[3].Item1);
        }


        [Fact]
        public void RaiseBeforeSaveTriggers_RecursiveAdd_RaisesSubsequentTriggers()
        {
            TestDbContext dbContext = null;

            var trigger = new TriggerStub<TestModel> {
                Priority = CommonTriggerPriority.Early,
                BeforeSaveHandler = (context, _) => {
                    if (context.Entity.Id == 1)
                    {
                        dbContext.TestModels.Add(new TestModel { Id = 2 });
                    }
                    return Task.CompletedTask;
                }
            };

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IBeforeSaveTrigger<TestModel>>(trigger)
                .AddTriggeredDbContext<TestDbContext>(options => {
                    options.UseInMemoryDatabase("Test");
                })
                .BuildServiceProvider();

            var scope = serviceProvider.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            var subject = CreateSubject(dbContext);

            dbContext.TestModels.Add(new TestModel { Id = 1 });

            subject.RaiseBeforeSaveTriggers();

            Assert.Equal(2, trigger.BeforeSaveInvocations.Count);
        }
    }
}
