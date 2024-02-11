using System;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerSessionSaveChangesInterceptorTests
    {
        public class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        class TestDbContext(bool stubService = true) : DbContext
        {
            readonly bool _stubService = stubService;
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
            => new(stubService);

        [Fact]
        public void SaveChanges_RaisesBeforeSaveChangesStartingTriggers()
        {
            var subject = new TestDbContext();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.SaveChanges();

            Assert.Equal(1, triggerServiceStub.LastSession.RaiseBeforeSaveStartingTriggersCalls);
        }


        [Fact]
        public void SaveChanges_RaisesBeforeSaveChangesCompletedTriggers()
        {
            var subject = new TestDbContext();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.SaveChanges();

            Assert.Equal(1, triggerServiceStub.LastSession.RaiseBeforeSaveCompletingTriggersCalls);
        }

        [Fact]
        public void SaveChanges_RaisesAfterSaveChangesStartingTriggers()
        {
            var subject = new TestDbContext();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.SaveChanges();

            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveStartingTriggersCalls);
        }


        [Fact]
        public void SaveChanges_RaisesAfterSaveChangesCompletedTriggers()
        {
            var subject = new TestDbContext();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            subject.SaveChanges();

            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveCompletedTriggersCalls);
        }

        [Fact]
        public void SaveChanges_OnDbUpdateException_RaisesAfterSaveFailedStartingTriggers()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var prepDbContext = new TestDbContext(false) { UseSqlLiteConnection = connection };
            prepDbContext.Database.EnsureCreated();
            var duplicatedId = Guid.NewGuid();
            prepDbContext.TestModels.Add(new TestModel { Id = duplicatedId });
            prepDbContext.SaveChanges();

            var subject = new TestDbContext() { UseSqlLiteConnection = connection };
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            // Inserting a model with the same Id should fail in the database
            subject.TestModels.Add(new TestModel {
                Id = duplicatedId,
                Name = "test1"
            });

            Assert.Throws<DbUpdateException>(() => subject.SaveChanges());
            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveFailedStartingTriggersCalls);
        }

        [Fact]
        public void SaveChanges_OnDbUpdateException_RaisesAfterSaveFailedCompletedTriggers()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var prepDbContext = new TestDbContext(false) { UseSqlLiteConnection = connection };
            prepDbContext.Database.EnsureCreated();
            var duplicatedId = Guid.NewGuid();
            prepDbContext.TestModels.Add(new TestModel { Id = duplicatedId });
            prepDbContext.SaveChanges();

            var subject = new TestDbContext() { UseSqlLiteConnection = connection };
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            // Inserting a model with the same Id should fail in the database
            subject.TestModels.Add(new TestModel {
                Id = duplicatedId,
                Name = "test1"
            });

            Assert.Throws<DbUpdateException>(() => subject.SaveChanges());
            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveFailedCompletedTriggersCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_RaisesBeforeSaveChangesStartingTriggers()
        {
            var subject = new TestDbContext();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            await subject.SaveChangesAsync();

            Assert.Equal(1, triggerServiceStub.LastSession.RaiseBeforeSaveStartingTriggersCalls);
        }


        [Fact]
        public async Task SaveChangesAsync_RaisesBeforeSaveChangesCompletedTriggers()
        {
            var subject = new TestDbContext();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            await subject.SaveChangesAsync();

            Assert.Equal(1, triggerServiceStub.LastSession.RaiseBeforeSaveCompletingTriggersCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_RaisesAfterSaveChangesStartingTriggers()
        {
            var subject = new TestDbContext();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            await subject.SaveChangesAsync();

            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveStartingTriggersCalls);
        }


        [Fact]
        public async Task SaveChangesAsync_RaisesAfterSaveChangesCompletedTriggers()
        {
            var subject = new TestDbContext();
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            await subject.SaveChangesAsync();

            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveCompletedTriggersCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_OnDbUpdateException_RaisesAfterSaveFailedStartingTriggers()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var prepDbContext = new TestDbContext(false) { UseSqlLiteConnection = connection };
            prepDbContext.Database.EnsureCreated();
            var duplicatedId = Guid.NewGuid();
            prepDbContext.TestModels.Add(new TestModel { Id = duplicatedId });
            prepDbContext.SaveChanges();

            var subject = new TestDbContext() { UseSqlLiteConnection = connection };
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            // Inserting a model with the same Id should fail in the database
            subject.TestModels.Add(new TestModel {
                Id = duplicatedId,
                Name = "test1"
            });

            await Assert.ThrowsAsync<DbUpdateException>(() => subject.SaveChangesAsync());
            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveFailedStartingTriggersCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_OnDbUpdateException_RaisesAfterSaveFailedCompletedTriggers()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var prepDbContext = new TestDbContext(false) { UseSqlLiteConnection = connection };
            prepDbContext.Database.EnsureCreated();
            var duplicatedId = Guid.NewGuid();
            prepDbContext.TestModels.Add(new TestModel { Id = duplicatedId });
            prepDbContext.SaveChanges();

            var subject = new TestDbContext() { UseSqlLiteConnection = connection };
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            // Inserting a model with the same Id should fail in the database
            subject.TestModels.Add(new TestModel {
                Id = duplicatedId,
                Name = "test1"
            });

            await Assert.ThrowsAsync<DbUpdateException>(() => subject.SaveChangesAsync());
            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveFailedCompletedTriggersCalls);
        }

        [Fact]
        public void SaveChanges_OnDbUpdateException_RaisesAfterSaveFailedTriggers()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var prepDbContext = new TestDbContext(false) { UseSqlLiteConnection = connection };
            prepDbContext.Database.EnsureCreated();
            var duplicatedId = Guid.NewGuid();
            prepDbContext.TestModels.Add(new TestModel { Id = duplicatedId });
            prepDbContext.SaveChanges();

            var subject = new TestDbContext() { UseSqlLiteConnection = connection };
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
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var prepDbContext = new TestDbContext(false) { UseSqlLiteConnection = connection };
            prepDbContext.Database.EnsureCreated();
            var duplicatedId = Guid.NewGuid();
            prepDbContext.TestModels.Add(new TestModel { Id = duplicatedId });
            prepDbContext.SaveChanges();

            var subject = new TestDbContext() { UseSqlLiteConnection = connection };
            var triggerServiceStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            // Inserting a model with the same Id should fail in the database
            subject.TestModels.Add(new TestModel {
                Id = duplicatedId,
                Name = "test1"
            });

            await Assert.ThrowsAsync<DbUpdateException>(async () => await subject.SaveChangesAsync());
            Assert.Equal(1, triggerServiceStub.LastSession.RaiseAfterSaveFailedTriggersCalls);
        }
    }
}
