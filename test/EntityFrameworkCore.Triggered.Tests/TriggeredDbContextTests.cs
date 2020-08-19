using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
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
            readonly bool _stubService;

            public TestDbContext(bool stubService)
            {
                _stubService = stubService;
            }

            public TestDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
            {
                _stubService = true;
            }

            public TriggerStub<TestModel> TriggerStub { get; } = new TriggerStub<TestModel>();

            public DbSet<TestModel> TestModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseTriggers(triggerOptions => {
                    triggerOptions.AddTrigger(TriggerStub);
                });

                if (_stubService)
                {
                    optionsBuilder.ReplaceService<ITriggerService, TriggerServiceStub>();
                }
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
    }
}
