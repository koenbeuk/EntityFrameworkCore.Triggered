using System;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using EntityFrameworkCore.Triggered.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests
{
#if EFCORETRIGGERED2
    public class EFCore5DbContextTests
    {
        public class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        class TestDbContext : DbContext
#pragma warning restore CS0618 // Type or member is obsolete
        {
            readonly bool _stubService;

            public TestDbContext(bool stubService = true)
            {
                _stubService = stubService;
            }

            public TriggerStub<TestModel> TriggerStub { get; } = new TriggerStub<TestModel>();

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.EnableServiceProviderCaching(false);
                optionsBuilder.UseTriggers(triggerOptions => {
                    triggerOptions.AddTrigger(TriggerStub);
                });

                if (_stubService)
                {
                    optionsBuilder.ReplaceService<ITriggerService, TriggerServiceStub>();
                }
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<TestModel>().HasKey(x => x.Id);

                base.OnModelCreating(modelBuilder);
            }
        }

        TestDbContext CreateSubject(bool stubService = true)
            => new(stubService);

        [Fact]
        public void SaveChanges_CreatesTriggerrSession()
        {
            var subject = CreateSubject();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.SaveChanges();
            Assert.Equal(1, triggerServiceStub.CreateSessionCalls);
        }

        [Fact]
        public void SaveChangesWithAccept_CreatesTriggerSession()
        {
            var subject = CreateSubject();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.SaveChanges(true);
            Assert.Equal(1, triggerServiceStub.CreateSessionCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_CreatesTriggerSession()
        {
            var subject = CreateSubject();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            await subject.SaveChangesAsync();
            Assert.Equal(1, triggerServiceStub.CreateSessionCalls);
        }

        [Fact]
        public void SaveChanges_ExplicitTriggerSession_ReturnsActiveTriggerSession()
        {
            var subject = CreateSubject(true);
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            using var triggerSession = subject.CreateTriggerSession();

            subject.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            subject.SaveChanges();

            Assert.Equal(1, triggerServiceStub.CreateSessionCalls);
        }

        [Fact]
        public void SaveChanges_RecursiveCall_ReturnsActiveTriggerSession()
        {
            var subject = CreateSubject(false);

            subject.TriggerStub.BeforeSaveHandler = (_1, _2) => {
                if (subject.TriggerStub.BeforeSaveInvocations.Count > 1)
                {
                    return Task.CompletedTask;
                }

                subject.SaveChanges();

                return Task.CompletedTask;
            };

            subject.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            subject.SaveChanges();

            Assert.Equal(1, subject.TriggerStub.BeforeSaveInvocations.Count);
        }

        [Fact]
        public async Task SaveChangesAsyncWithAccept_CreatesChangeHandlerSession()
        {
            var subject = CreateSubject();
            var triggerSessionStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            await subject.SaveChangesAsync(true);
            Assert.Equal(1, triggerSessionStub.CreateSessionCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_RecursiveCall_ReturnsActiveTriggerSession()
        {
            var subject = CreateSubject(false);

            subject.TriggerStub.BeforeSaveHandler = (_1, _2) => {
                if (subject.TriggerStub.BeforeSaveInvocations.Count > 1)
                {
                    return Task.CompletedTask;
                }

                return subject.SaveChangesAsync();
            };

            subject.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            await subject.SaveChangesAsync();

            Assert.Equal(1, subject.TriggerStub.BeforeSaveInvocations.Count);
        }

        [Fact]
        public void SetTriggerServiceProvider_CallsCapturedChanges()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var subject = new TestDbContext();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            subject.SaveChanges();

            Assert.Equal(1, triggerServiceStub.LastSession.CaptureDiscoveredChangesCalls);
        }
    }
#endif
}
