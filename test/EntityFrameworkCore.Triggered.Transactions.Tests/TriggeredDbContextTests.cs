using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Transactions.Tests.Stubs;
using EntityFrameworkCore.Triggered.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EntityFrameworkCore.Triggered.Transactions.Tests
{
    public class TriggeredDbContextTests 
    {
        class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

#pragma warning disable CS0618 // Type or member is obsolete
        class TestDbContext : TriggeredDbContext
#pragma warning restore CS0618 // Type or member is obsolete
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
                optionsBuilder.EnableServiceProviderCaching(false);
                optionsBuilder.UseTriggers(triggerOptions => {
                    triggerOptions
                        .UseTransactionTriggers()
                        .AddTrigger(TriggerStub);
                });
            }
        }

        [Fact]
        public void RaiseBeforeCommitTriggers_DiscoveredChangesFromTriggeredDbContext_CallsTriggers()
        {
            var dbContext = new TestDbContext();
            using var subject = dbContext.CreateTriggerSession();
            
            dbContext.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            dbContext.SaveChanges();

            subject.RaiseBeforeCommitTriggers().GetAwaiter().GetResult();

            Assert.Equal(1, dbContext.TriggerStub.BeforeCommitInvocations.Count);
        }


        [Fact]
        public void RaiseAfterCommitTriggers_DiscoveredChangesFromTriggeredDbContext_CallsTriggers()
        {
            var dbContext = new TestDbContext();
            using var subject = dbContext.CreateTriggerSession();

            dbContext.TestModels.Add(new TestModel {
                Id = Guid.NewGuid(),
                Name = "test1"
            });

            dbContext.SaveChanges();

            subject.RaiseAfterCommitTriggers().GetAwaiter().GetResult();

            Assert.Equal(1, dbContext.TriggerStub.AfterCommitInvocations.Count);
        }
    }
}
