using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests
{
    public class ChangeEventDbContextTests
    {
        class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        class TestDbContext : ChangeEventDbContext
        {
            public ChangeEventHandlerStub<TestModel> ChangeEventHandlerStub { get; } = new ChangeEventHandlerStub<TestModel>();

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseEvents(eventOptions => {
                    eventOptions.AddChangeEventHandler(ChangeEventHandlerStub);
                });
                optionsBuilder.ReplaceService<IChangeEventService, ChangeEventServiceStub>();
            }
        }

        TestDbContext CreateSubject()
            => new TestDbContext();

        [Fact]
        public void SaveChanges_CreatesChangeHandlerSession()
        {
            var subject = CreateSubject();
            var changeEventSessionStub = (ChangeEventServiceStub)subject.GetService<IChangeEventService>();

            subject.SaveChanges();
            Assert.Equal(1, changeEventSessionStub.CreateSessionCalls);
        }

        [Fact]
        public void SaveChangesWithAccept_CreatesChangeHandlerSession()
        {
            var subject = CreateSubject();
            var changeEventSessionStub = (ChangeEventServiceStub)subject.GetService<IChangeEventService>();

            subject.SaveChanges(true);
            Assert.Equal(1, changeEventSessionStub.CreateSessionCalls);
        }

        [Fact]
        public async Task SaveChangesAsync_CreatesChangeHandlerSession()
        {
            var subject = CreateSubject();
            var changeEventSessionStub = (ChangeEventServiceStub)subject.GetService<IChangeEventService>();

            await subject.SaveChangesAsync();
            Assert.Equal(1, changeEventSessionStub.CreateSessionCalls);
        }

        [Fact]
        public async Task SaveChangesAsyncWithAccept_CreatesChangeHandlerSession()
        {
            var subject = CreateSubject();
            var changeEventSessionStub = (ChangeEventServiceStub)subject.GetService<IChangeEventService>();

            await subject.SaveChangesAsync(true);
            Assert.Equal(1, changeEventSessionStub.CreateSessionCalls);
        }
    }
}
