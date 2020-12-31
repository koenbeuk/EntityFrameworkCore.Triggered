using System;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Transactions.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace EntityFrameworkCore.Triggered.Transactions.Tests
{
    public class TriggeredSessionExtensionsTests
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
                
                optionsBuilder.ConfigureWarnings(warningOptions => {
                    warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                });
                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseTriggers(triggerOptions => {
                    triggerOptions
                        .UseTransactionTriggers()
                        .AddTrigger(TriggerStub);
                });
            }
        }

        protected static ITriggerSession CreateSession(DbContext context)
            => context.Database.GetService<ITriggerService>().CreateSession(context);


        [Fact]
        public async Task RaiseBeforeCommitTriggers_RaisesNothingOnNoChanges()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            session.DiscoverChanges();
            await session.RaiseBeforeCommitTriggers();

            Assert.Empty(context.TriggerStub.BeforeCommitInvocations);
        }

        [Fact]
        public async Task RaiseBeforeCommitTriggers_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            context.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            session.DiscoverChanges();
            await session.RaiseBeforeCommitTriggers();

            Assert.Equal(1, context.TriggerStub.BeforeCommitInvocations.Count);
        }

        [Fact]
        public async Task RaiseAfterCommitTriggers_RaisesNothingOnNoChanges()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            session.DiscoverChanges();
            await session.RaiseAfterCommitTriggers();

            Assert.Empty(context.TriggerStub.AfterCommitInvocations);
        }

        [Fact]
        public async Task RaiseAfterCommitTriggers_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            context.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            session.DiscoverChanges();
            await session.RaiseAfterCommitTriggers();

            Assert.Equal(1, context.TriggerStub.AfterCommitInvocations.Count);
        }


        [Fact]
        public async Task RaiseBeforeRollbackTriggers_RaisesNothingOnNoChanges()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            session.DiscoverChanges();
            await session.RaiseBeforeRollbackTriggers();

            Assert.Empty(context.TriggerStub.BeforeRollbackInvocations);
        }

        [Fact]
        public async Task RaiseBeforeRollbackTriggers_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            context.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            session.DiscoverChanges();
            await session.RaiseBeforeRollbackTriggers();

            Assert.Equal(1, context.TriggerStub.BeforeRollbackInvocations.Count);
        }


        [Fact]
        public async Task RaiseAfterRollbackTriggers_RaisesNothingOnNoChanges()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            session.DiscoverChanges();
            await session.RaiseAfterRollbackTriggers();

            Assert.Empty(context.TriggerStub.AfterRollbackInvocations);
        }

        [Fact]
        public async Task RaiseAfterRollbackTriggers_RaisesOnceOnSimpleAddition()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            context.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            session.DiscoverChanges();
            await session.RaiseAfterRollbackTriggers();

            Assert.Equal(1, context.TriggerStub.AfterRollbackInvocations.Count);
        }


        [Fact]
        public void RaiseBeforeCommitStartingTriggers_CallsTriggers()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            session.RaiseBeforeCommitStartingTriggers().GetAwaiter().GetResult();

            Assert.Equal(1, context.TriggerStub.BeforeCommitStartingInvocationsCount);
        }

        [Fact]
        public void RaiseBeforeCommitCompletedTriggers_CallsTriggers()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            session.RaiseBeforeCommitCompletedTriggers().GetAwaiter().GetResult();

            Assert.Equal(1, context.TriggerStub.BeforeCommitCompletedInvocationsCount);
        }

        [Fact]
        public void RaiseAfterCommitStartingTriggers_CallsTriggers()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            session.RaiseAfterCommitStartingTriggers().GetAwaiter().GetResult();

            Assert.Equal(1, context.TriggerStub.AfterCommitStartingInvocationsCount);
        }

        [Fact]
        public void RaiseAfterCommitCompletedTrigger_CallsTriggers()
        {
            using var context = new TestDbContext();
            var session = CreateSession(context);

            session.RaiseAfterCommitCompletedTriggers().GetAwaiter().GetResult();

            Assert.Equal(1, context.TriggerStub.AfterCommitCompletedInvocationsCount);
        }
    }
}
