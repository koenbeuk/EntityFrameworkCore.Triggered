﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests
{
    public class TriggeredDbContextFactoryTests
    {
        public class TestModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        class TestTrigger(TriggeredDbContextFactoryTests.TestDbContext testDbContext) : IBeforeSaveTrigger<TestModel>
        {
            readonly TestDbContext _testDbContext = testDbContext;

            public void BeforeSave(ITriggerContext<TestModel> context) => _testDbContext.TriggerRaised = true;
        }

        class TestDbContext(DbContextOptions options) : DbContext(options)
        {
            public DbSet<TestModel> TestModels { get; set; }

            public bool TriggerRaised { get; set; } = false;
        }

        [Fact]
        public void DbContextFactory_RaisesTrigger_SharesDbContext()
        {
            using var serviceProvider = new ServiceCollection()
                .AddTriggeredDbContextFactory<TestDbContext>(options => {
                    options.UseInMemoryDatabase(nameof(DbContextFactory_RaisesTrigger_SharesDbContext));
                    options.UseTriggers(triggerOptions => {
                        triggerOptions.AddTrigger<TestTrigger>();
                    });
                    options.ConfigureWarnings(warningOptions => {
                        warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                    });
                })
                .BuildServiceProvider();

            using var serviceScope = serviceProvider.CreateScope();
            var dbContextFactory = serviceScope.ServiceProvider.GetService<IDbContextFactory<TestDbContext>>();
            using var dbContext = dbContextFactory.CreateDbContext();

            dbContext.TestModels.Add(new TestModel { });
            dbContext.SaveChanges();

            Assert.True(dbContext.TriggerRaised);
        }
    }
}
