using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Infrastructure
{
    public class ServiceCollectionExtensionsTests
    {
        class TestModel { public int Id { get; set; } }
#pragma warning disable CS0618 // Type or member is obsolete
        class TestDbContext : TriggeredDbContext
#pragma warning restore CS0618 // Type or member is obsolete
        {

            public DbSet<TestModel> TestModels { get; set; }
            public TestDbContext(DbContextOptions options) : base(options)
            {
            }
        }

        [Fact]
        public void AddTriggeredDbContext_AddsTriggersAndCallsUsersAction()
        {
            var subject = new ServiceCollection();
            var optionsActionsCalled = false;
            subject.AddTriggeredDbContext<TestDbContext>(options => {
                optionsActionsCalled = true;
                options.UseInMemoryDatabase("test");
                options.EnableServiceProviderCaching(false);
            });

            var serviceProvider = subject.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<TestDbContext>();

            Assert.True(optionsActionsCalled);
            Assert.NotNull(context.GetService<ITriggerService>());
        }

        [Fact]
        public void AddTriggeredDbContextPool_AddsTriggersAndCallsUsersAction()
        {
            var subject = new ServiceCollection();
            var optionsActionsCalled = false;
            subject.AddTriggeredDbContext<TestDbContext>(options => {
                optionsActionsCalled = true;
                options.UseInMemoryDatabase("test");
                options.EnableServiceProviderCaching(false);
            });

            var serviceProvider = subject.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<TestDbContext>();

            Assert.True(optionsActionsCalled);
            Assert.NotNull(context.GetService<ITriggerService>());
        }


        [Fact]
        public void AddTriggeredDbContext_ReusesScopedServiceProvider()
        {
            var subject = new ServiceCollection();
            subject.AddTriggeredDbContext<TestDbContext>(options => {
                options.UseInMemoryDatabase("test");
                options.EnableServiceProviderCaching(false);
            }).AddScoped<IBeforeSaveTrigger<TestModel>, TriggerStub<TestModel>>();

            var serviceProvider = subject.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            context.TestModels.Add(new TestModel());

            context.SaveChanges();

            var triggerStub = scope.ServiceProvider.GetRequiredService<IBeforeSaveTrigger<TestModel>>() as TriggerStub<TestModel>;
            Assert.NotNull(triggerStub);
            Assert.Equal(1, triggerStub.BeforeSaveInvocations.Count);
        }


        [Fact]
        public void AddTriggeredDbContextPool_ReusesScopedServiceProvider()
        {
            var subject = new ServiceCollection();
            subject.AddTriggeredDbContextPool<TestDbContext>(options => {
                options.UseInMemoryDatabase("test");
                options.EnableServiceProviderCaching(false);
            }).AddScoped<IBeforeSaveTrigger<TestModel>, TriggerStub<TestModel>>();

            var serviceProvider = subject.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
            context.TestModels.Add(new TestModel());

            context.SaveChanges();

            var triggerStub = scope.ServiceProvider.GetRequiredService<IBeforeSaveTrigger<TestModel>>() as TriggerStub<TestModel>;
            Assert.NotNull(triggerStub);
            Assert.Equal(1, triggerStub.BeforeSaveInvocations.Count);
        }


#if EFCORETRIGGERED2
        [Fact]
        public void AddTriggeredDbContextFactory_ReusesScopedServiceProvider()
        {
            var subject = new ServiceCollection();
            subject.AddTriggeredDbContextFactory<TestDbContext>(options => {
                options.UseInMemoryDatabase("test");
                options.EnableServiceProviderCaching(false);
            }).AddScoped<IBeforeSaveTrigger<TestModel>, TriggerStub<TestModel>>();

            var serviceProvider = subject.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TestDbContext>>();
            var context = contextFactory.CreateDbContext();

            context.TestModels.Add(new TestModel());

            context.SaveChanges();

            var triggerStub = scope.ServiceProvider.GetRequiredService<IBeforeSaveTrigger<TestModel>>() as TriggerStub<TestModel>;
            Assert.NotNull(triggerStub);
            Assert.Equal(1, triggerStub.BeforeSaveInvocations.Count);
        }

        [Fact]
        public void AddTriggeredPooledDbContextFactory_ReusesScopedServiceProvider()
        {
            var subject = new ServiceCollection();
            subject.AddTriggeredPooledDbContextFactory<TestDbContext>(options => {
                options.UseInMemoryDatabase("test");
                options.EnableServiceProviderCaching(false);
            }).AddScoped<IBeforeSaveTrigger<TestModel>, TriggerStub<TestModel>>();

            var serviceProvider = subject.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TestDbContext>>();
            var context = contextFactory.CreateDbContext();

            context.TestModels.Add(new TestModel());

            context.SaveChanges();

            var triggerStub = scope.ServiceProvider.GetRequiredService<IBeforeSaveTrigger<TestModel>>() as TriggerStub<TestModel>;
            Assert.NotNull(triggerStub);
            Assert.Equal(1, triggerStub.BeforeSaveInvocations.Count);
        }
#endif
    }
}
