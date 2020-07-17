using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests
{
    public class TriggeredDbContextTests
    {
        class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        class TestDbContext : TriggeredDbContext
        {
            public TriggerStub<TestModel> TriggerStub { get; } = new TriggerStub<TestModel>();

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseTriggers(triggerOptions => {
                    triggerOptions.AddTrigger(TriggerStub);
                });
                optionsBuilder.ReplaceService<ITriggerService, TriggerServiceStub>();
            }
        }

        TestDbContext CreateSubject()
            => new TestDbContext();

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
        public async Task SaveChangesAsyncWithAccept_CreatesChangeHandlerSession()
        {
            var subject = CreateSubject();
            var TriggersessionStub = (TriggerServiceStub)subject.GetService<ITriggerService>();

            await subject.SaveChangesAsync(true);
            Assert.Equal(1, TriggersessionStub.CreateSessionCalls);
        }
    }
}
