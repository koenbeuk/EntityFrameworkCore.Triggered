using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ScenarioTests;
using Xunit;

namespace EntityFrameworkCore.Triggered.IntegrationTests.OwnedEntities
{
    public partial class OwnedEntities
    {
        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public void TestScenario(ScenarioContext scenario)
        {
            var dbContext = new ApplicationDbContext(scenario.TargetName);

            Assert.Equal(0, dbContext.OwneeTrigger.Calls);
            Assert.Equal(0, dbContext.OwnerTrigger.Calls);

            var owner = new Owner {
                Ownee = new Ownee()
            };

            dbContext.Add(owner);

            scenario.Fact("We can trigger both ownee and owner on save", () => {
                dbContext.SaveChanges();

                Assert.Equal(1, dbContext.OwneeTrigger.Calls);
                Assert.Equal(1, dbContext.OwnerTrigger.Calls);
            });

            dbContext.SaveChanges();

            scenario.Fact("We can update the ownee and trigger that", () => {
                owner.Ownee.Name = "something new";
                dbContext.SaveChanges();

                Assert.Equal(2, dbContext.OwneeTrigger.Calls);
                Assert.Equal(1, dbContext.OwnerTrigger.Calls);
            });

            scenario.Fact("We can update the owner and trigger that", () => {
                owner.Name = "something new";
                dbContext.SaveChanges();

                Assert.Equal(1, dbContext.OwneeTrigger.Calls);
                Assert.Equal(2, dbContext.OwnerTrigger.Calls);
            });
        }
    }
}
