using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Extensions
{
    public class DbContextExtensionTests
    {
        class TestModel { public int Id { get; set; } public string Name { get; set; } }

        class SampleTrigger : IBeforeSaveTrigger<TestModel>
        {
            public int BeforeSaveCalls { get; set; }

            public void BeforeSave(ITriggerContext<TestModel> context)
            {
                BeforeSaveCalls += 1;
            }
        }

        class TestDbContext : DbContext
        {
            public bool UseTriggers { get; set; } = true;

            public DbSet<TestModel> TestModels { get; set; }

            public SampleTrigger SampleTrigger { get; } = new();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("test");
                if (UseTriggers)
                {
                    optionsBuilder.UseTriggers(triggerOptions => {
                        triggerOptions.AddTrigger(SampleTrigger);
                    });
                }

                optionsBuilder.ConfigureWarnings(warningOptions => {
                    warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                });
            }
        }

        [Fact]
        public void GetTriggerService_ValidDbContext_ReturnsSession()
        {
            using var context = new TestDbContext();

            var triggerService = DbContextExtensions.GetTriggerService(context);

            Assert.NotNull(triggerService);
        }


        [Fact]
        public void GetTriggerService_NonTriggeredDbContext_ReturnsSession()
        {
            using var context = new TestDbContext { UseTriggers = false };

            Assert.Throws<InvalidOperationException>(() =>
                DbContextExtensions.GetTriggerService(context));
        }

        [Fact]
        public void CreateTriggerSession_ValidDbContext_CreatesNewSession()
        {
            using var context = new TestDbContext();
            var triggerSession = DbContextExtensions.CreateTriggerSession(context);

            Assert.NotNull(triggerSession);
            Assert.NotNull(context.GetService<ITriggerService>()?.Current);
        }

        [Fact]
        public void SaveChangesWithoutTriggers_DoesNotRaiseTrigger()
        {
            // arrange
            using var context = new TestDbContext();
            context.TestModels.Add(new TestModel { });

            // act
            context.SaveChangesWithoutTriggers();

            // assert
            Assert.Equal(0, context.SampleTrigger.BeforeSaveCalls);
        }

        [Fact]
        public void SaveChangesWithoutTriggers_RestoresConfiguration()
        {
            // arrange
            using var context = new TestDbContext();
            context.TestModels.Add(new TestModel { });
            var expectedConfiguration = context.GetTriggerService().Configuration;

            // act
            context.SaveChangesWithoutTriggers();

            // assert
            Assert.Same(expectedConfiguration, context.GetTriggerService().Configuration);
        }

        [Fact]
        public async Task SaveChangesAsyncWithoutTriggers_DoesNotRaiseTrigger()
        {
            // arrange
            using var context = new TestDbContext();
            context.TestModels.Add(new TestModel { });

            // act
            await context.SaveChangesWithoutTriggersAsync();

            // assert
            Assert.Equal(0, context.SampleTrigger.BeforeSaveCalls);
        }

        [Fact]
        public async Task SaveChangesAsyncWithoutTriggers_RestoresConfiguration()
        {
            // arrange
            using var context = new TestDbContext();
            context.TestModels.Add(new TestModel { });
            var expectedConfiguration = context.GetTriggerService().Configuration;

            // act
            await context.SaveChangesWithoutTriggersAsync();

            // assert
            Assert.Same(expectedConfiguration, context.GetTriggerService().Configuration);
        }


        [Fact]
        public void CreateNewTriggerSession_ExistingSession_Throws()
        {
            // arrange
            using var context = new TestDbContext();
            context.TestModels.Add(new TestModel { });
            context.CreateNewTriggerSession();

            // act
            Assert.Throws<InvalidOperationException>(() =>
                context.CreateNewTriggerSession());
        }


    }
}
