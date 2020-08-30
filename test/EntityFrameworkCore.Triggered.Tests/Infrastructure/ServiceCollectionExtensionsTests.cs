using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Infrastructure
{
    public class ServiceCollectionExtensionsTests
    {

        class TestDbContext : TriggeredDbContext
        {
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
            });

            var serviceProvider = subject.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<TestDbContext>();

            Assert.True(optionsActionsCalled);
            Assert.NotNull(context.GetService<ITriggerService>());
        }
    }
}
