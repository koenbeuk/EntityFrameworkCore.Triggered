using System;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests
{
    public class TriggeredDbContextTests
    {
        public class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        class TestDbContext : TriggeredDbContext
#pragma warning restore CS0618 // Type or member is obsolete
        {
            readonly bool _stubService;

            public TestDbContext(bool stubService = true)
            {
                _stubService = stubService;
            }

            public TestDbContext(IServiceProvider serviceProvider, bool stubService = true) : base(serviceProvider)
            {
                _stubService = stubService;
            }

            public SqliteConnection UseSqlLiteConnection;

            public TriggerStub<TestModel> TriggerStub { get; } = new TriggerStub<TestModel>();

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.ConfigureWarnings(warningOptions => {
                    warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                });

                if (UseSqlLiteConnection != null)
                {
                    optionsBuilder.UseSqlite(UseSqlLiteConnection);
                }
                else
                {
                    optionsBuilder.UseInMemoryDatabase("test");
                }
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
            => new TestDbContext(stubService);

        [Fact]
        public void SaveChanges_CreatesChangeHandlerSession()
        {
            var subject = CreateSubject();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.SaveChanges();
            Assert.Equal(1, triggerServiceStub.CreateSessionCalls);
        }

        [Fact]
        public void SaveChangesWithAccept_CreatesChangeHandlerSession()
        {
            var subject = CreateSubject();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.SaveChanges(true);
            Assert.Equal(1, triggerServiceStub.CreateSessionCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_CreatesChangeHandlerSession()
        {
            var subject = CreateSubject();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            await subject.SaveChangesAsync();
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
        public void SaveChanges_CapturedServiceProvider_Forwards()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var subject = new TestDbContext(serviceProvider);
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            subject.SaveChanges();

            Assert.Equal(serviceProvider, triggerServiceStub.ServiceProvider);
        }


        [Fact]
        public void SaveChanges_CallsCapturedChanges()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var subject = new TestDbContext(serviceProvider);
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            subject.SaveChanges();

            Assert.Equal(1, triggerServiceStub.LastSession.CaptureDiscoveredChangesCalls);
        }

        [Fact]
        public async Task SaveChangesAysnc_CallsCapturedChanges()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var subject = new TestDbContext(serviceProvider);
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            await subject.SaveChangesAsync();

            Assert.Equal(1, triggerServiceStub.LastSession.CaptureDiscoveredChangesCalls);
        }

        [Fact]
        public void SaveChanges_OnDbUpdateException_RaisesAfterSaveFailedTriggers()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var prepDbContext = new TestDbContext(false) { UseSqlLiteConnection = connection };
            prepDbContext.Database.EnsureCreated();
            var duplicatedId = Guid.NewGuid();
            prepDbContext.TestModels.Add(new TestModel { Id = duplicatedId });
            prepDbContext.SaveChanges();

            var subject = new TestDbContext(serviceProvider) { UseSqlLiteConnection = connection };
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            // Inserting a model with the same Id should fail in the database
            subject.TestModels.Add(new TestModel {
                Id = duplicatedId,
                Name = "test1"
            });

            Assert.Throws<DbUpdateException>(() => subject.SaveChanges());
            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveFailedTriggersCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_OnDbUpdateException_RaisesAfterSaveFailedTriggers()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var prepDbContext = new TestDbContext(false) { UseSqlLiteConnection = connection };
            prepDbContext.Database.EnsureCreated();
            var duplicatedId = Guid.NewGuid();
            prepDbContext.TestModels.Add(new TestModel { Id = duplicatedId });
            prepDbContext.SaveChanges();

            var subject = new TestDbContext(serviceProvider) { UseSqlLiteConnection = connection };
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            // Inserting a model with the same Id should fail in the database
            subject.TestModels.Add(new TestModel {
                Id = duplicatedId,
                Name = "test1"
            });

            await Assert.ThrowsAsync<DbUpdateException>(async () => await subject.SaveChangesAsync());
            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveFailedTriggersCalls);
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


        [Fact]
        public void SaveChanges_DisposesTriggerSession()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var subject = new TestDbContext(serviceProvider);
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();
                    
            subject.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            subject.SaveChanges();

            Assert.Equal(1, triggerServiceStub.LastSession.DisposeCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_DisposesTriggerSession()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var subject = new TestDbContext(serviceProvider);
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            await subject.SaveChangesAsync();

            Assert.Equal(1, triggerServiceStub.LastSession.DisposeCalls);
        }
    }
}
