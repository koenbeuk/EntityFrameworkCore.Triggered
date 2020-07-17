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
    public class TriggerSessionTests
    {
        class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        class TestDbContext : DbContext
        {
            public TriggerStub<TestModel> TriggerStub { get; } = new TriggerStub<TestModel>();

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseTriggers(triggerOptions =>
                {
                    triggerOptions.AddTrigger(TriggerStub);
                });
            }
        }

        protected ITriggerSession CreateSubject(DbContext context)
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

            await subject.RaiseBeforeSaveTriggers();
            await subject.RaiseAfterSaveTriggers();

            Assert.Empty(context.TriggerStub.AfterSaveInvocations);
        }

        [Fact]
        public async Task RaiseBeforeSaveTriggers_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel
            {
                Id = Guid.NewGuid(),
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
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            await Assert.ThrowsAsync<InvalidOperationException>(() => subject.RaiseAfterSaveTriggers());
        }

        [Fact]
        public async Task RaiseAfterSaveTriggers_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel
            {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            await subject.RaiseBeforeSaveTriggers();
            await subject.RaiseAfterSaveTriggers();

            Assert.Equal(1, context.TriggerStub.AfterSaveInvocations.Count);
        }

        [Fact]
        public async Task RaiseBeforeSaveTriggers_ThrowsOnCancelledException()
        {
            using var context = new TestDbContext();
            var subject = CreateSubject(context);

            context.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
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
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            var cancellationTokenSource = new CancellationTokenSource();
            await subject.RaiseBeforeSaveTriggers();

            cancellationTokenSource.Cancel();
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await subject.RaiseAfterSaveTriggers(cancellationTokenSource.Token));
        }
    }
}
