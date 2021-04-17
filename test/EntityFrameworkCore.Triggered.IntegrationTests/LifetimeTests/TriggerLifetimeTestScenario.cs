using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScenarioTests;
using Xunit;

namespace EntityFrameworkCore.Triggered.IntegrationTests.LifetimeTests
{
    public partial class TriggerLifetimeTestScenario
    {
        public int SingletonTriggerInstances { get; set; }
        public int ScopedTriggerInstances { get; set; }
        public int TransientTriggerInstances { get; set; }

        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public void Scenario(ScenarioContext scenario)
        {
            const int iterations = 5;
            const int usersPerIteration = 5;

            using var serviceProvider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(options => {
                    options.UseInMemoryDatabase(scenario.TargetName);
                    options.UseTriggers(triggerOptions => {
                        triggerOptions.AddTrigger<Triggers.Users.SingletonTrigger>(ServiceLifetime.Singleton);
                        triggerOptions.AddTrigger<Triggers.Users.ScopedTrigger>(ServiceLifetime.Scoped);
                        triggerOptions.AddTrigger<Triggers.Users.TransientTrigger>();
                    });
                })
                .AddSingleton(this)
                .BuildServiceProvider();

            for (var iteration = 0; iteration < iterations; iteration++)
            {
                using var serviceScope = serviceProvider.CreateScope();

                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                for (var i = 0; i < usersPerIteration; i++)
                {
                    dbContext.Users.Add(new User { });
                }

                dbContext.SaveChanges();
            }

            scenario.Fact("1: There is only 1 singleton trigger instance", () => {
                Assert.Equal(1, SingletonTriggerInstances);
            });

            scenario.Fact("2: There are 5 scoped trigger instances", () => {
                Assert.Equal(5, ScopedTriggerInstances);
            });

            scenario.Fact("3: There are 25 transient trigger instances", () => {
                Assert.Equal(25, TransientTriggerInstances);
            });
        }
    }
}
