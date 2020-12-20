using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Tests.Stubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.InMemory.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class ApplicationTriggerServiceProviderAccessorTests
    {
        class TestDbContext : DbContext
        {
            public TestDbContext()
            {

            }

            public TestDbContext(DbContextOptions options) : base(options)
            {
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                base.OnConfiguring(optionsBuilder);

                optionsBuilder.UseInMemoryDatabase("test");
                optionsBuilder.UseTriggers(triggerOptions => {
                    triggerOptions.AddTrigger<TriggerStub<object>>();
                });

                optionsBuilder.ConfigureWarnings(warningOptions => {
                    warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                });
            }
        }


        [Fact]
        public void GetTriggerServiceProvider_NoApplicationDi_ReturnsScopedInternal()
        {
            var dbContext = new TestDbContext();

            var subject = dbContext.GetInfrastructure().GetRequiredService<ApplicationTriggerServiceProviderAccessor>();
            var scopedObject1 = subject.GetTriggerServiceProvider().GetService<IBeforeSaveTrigger<object>>();
            Assert.NotNull(scopedObject1);

            var scopedObject2 = subject.GetTriggerServiceProvider().GetService<IBeforeSaveTrigger<object>>();
            Assert.NotNull(scopedObject2);

            Assert.NotEqual(scopedObject1, scopedObject2);
        }

        [Fact]
        public void GetTriggerServiceProvider_WithApplicationDiAndTransform_ReturnsCustomServiceProvider()
        {
            var applicationServiceProvider = new ServiceCollection()
                .AddDbContext<TestDbContext>(options => {
                    options.ConfigureWarnings(warningOptions => {
                        warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                    });

                    options.UseInMemoryDatabase("Test")
                           .UseTriggers();
                })
                .AddScoped<object>()
                .BuildServiceProvider();

            var dbContext = applicationServiceProvider.GetRequiredService<TestDbContext>();

            var subject = new ApplicationTriggerServiceProviderAccessor(dbContext.GetInfrastructure(), _ => applicationServiceProvider, new NullLogger<ApplicationTriggerServiceProviderAccessor>());
            var triggerServiceProvider = subject.GetTriggerServiceProvider();

            Assert.Equal(applicationServiceProvider, triggerServiceProvider);
        }

        [Fact]
        public void GetTriggerServiceProvider_WithExplicitlySetServiceProvider_ReturnsSetServiceProvider()
        {
            var applicationServiceProvider = new ServiceCollection()
                .AddDbContext<TestDbContext>(options => {
                    options.ConfigureWarnings(warningOptions => {
                        warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                    });

                    options.UseInMemoryDatabase("Test")
                           .UseTriggers();
                })
                .AddScoped<object>()
                .BuildServiceProvider();

            var internalServiceProviderStub = new ServiceCollection().BuildServiceProvider();

            var subject = applicationServiceProvider.GetRequiredService<TestDbContext>().GetService<ApplicationTriggerServiceProviderAccessor>();
            subject.SetTriggerServiceProvider(applicationServiceProvider);
            
            var triggerServiceProvider = subject.GetTriggerServiceProvider();

            Assert.Equal(applicationServiceProvider, triggerServiceProvider);
        }

    }
}
