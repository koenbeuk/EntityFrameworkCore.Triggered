using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Transactions.Tests.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.AspNetCore.Tests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        public class TestModel
        {
            public Guid Id { get; set; }
        }

        public class TestDbContext : TriggeredDbContext
        {
            public TestDbContext(DbContextOptions options) : base(options)
            {
            }

            public DbSet<TestModel> TestModels { get; set; }
        }


        [Fact]
        public void AddTriggeredDbContext_RegistersHttpContextServiceProviderAccessor()
        {
            IServiceProvider capturedServiceProvider = null;

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IHttpContextAccessor, Stubs.HttpContextAccessorStub>()
                .AddAspNetCoreTriggeredDbContext<TestDbContext>(options => {
                    options.UseInMemoryDatabase("Test");
                })
                .AddTransient<IBeforeSaveTrigger<TestModel>>(serviceProvider => {
                    capturedServiceProvider = serviceProvider;
                    return new TriggerStub<TestModel>();
                })
                .BuildServiceProvider();


            using var serviceScope = serviceProvider.CreateScope();
            serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.RequestServices = serviceScope.ServiceProvider;
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

            dbContext.Add(new TestModel { });
            dbContext.SaveChanges();

            Assert.NotNull(capturedServiceProvider);
            Assert.Equal(serviceScope.ServiceProvider, capturedServiceProvider);
        }
    }
}
