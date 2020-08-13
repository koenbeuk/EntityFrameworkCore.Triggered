using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests
{
    public class PerfTest
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

        [Fact]
        public void Test()
        {
            const int innerLoop = 1000;

            var subject = new TestDbContext(false);

            for (var i = 0; i < innerLoop; i++)
            {
                subject.TestModels.Add(new TestModel());
                subject.SaveChanges();
            }
        }

    }
}
