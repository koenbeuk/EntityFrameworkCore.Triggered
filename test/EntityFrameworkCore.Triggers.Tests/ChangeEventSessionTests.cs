using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests
{
    public class ChangeEventSessionTests
    {
        class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        class TestDbContext : DbContext
        {
            public ChangeEventHandlerStub<TestModel> ChangeEventHandlerStub { get; } = new ChangeEventHandlerStub<TestModel>();

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseEvents(eventOptions =>
                {
                    eventOptions.AddChangeEventHandler(ChangeEventHandlerStub);
                });
            }
        }

        protected IChangeEventSession CreateSubject(DbContext context)
            => context.Database.GetService<IChangeEventService>().CreateSession(context);

        [Fact]
        public async Task RaiseBeforeSaveChangeEvents_RaisesNothingOnNoChanges()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            await subject.RaiseBeforeSaveChangeEvents();

            Assert.Empty(context.ChangeEventHandlerStub.BeforeSaveInvocations);
        }

        [Fact]
        public async Task RaiseAfterSaveChangeEvents_RaisesNothingOnNoChanges()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            await subject.RaiseBeforeSaveChangeEvents();
            await subject.RaiseAfterSaveChangeEvents();

            Assert.Empty(context.ChangeEventHandlerStub.AfterSaveInvocations);
        }

        [Fact]
        public async Task RaiseBeforeSaveChangeEvents_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel
            {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            await subject.RaiseBeforeSaveChangeEvents();

            Assert.Equal(1, context.ChangeEventHandlerStub.BeforeSaveInvocations.Count);
        }

        [Fact]
        public async Task RaiseAfterSaveChangeEvents_WithoutCallToRaiseBeforeSaveChangeEvents_Throws()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => subject.RaiseAfterSaveChangeEvents());
        }

        [Fact]
        public async Task RaiseAfterSaveChangeEvents_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel
            {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            await subject.RaiseBeforeSaveChangeEvents();
            await subject.RaiseAfterSaveChangeEvents();

            Assert.Equal(1, context.ChangeEventHandlerStub.AfterSaveInvocations.Count);
        }

        [Fact]
        public async Task RaiseBeforeSaveChangeEvents_ThrowsOnCancelledException()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await subject.RaiseBeforeSaveChangeEvents(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task RaiseAfterSaveChangeEvents_ThrowsOnCancelledException()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            var cancellationTokenSource = new CancellationTokenSource();
            await subject.RaiseBeforeSaveChangeEvents();

            cancellationTokenSource.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await subject.RaiseAfterSaveChangeEvents(cancellationTokenSource.Token));
        }
    }
}
