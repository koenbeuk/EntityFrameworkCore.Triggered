using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes.Models;
using ScenarioTests;
using Xunit;

namespace EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes
{
    public partial class CascadingSoftDeletesTestScenario
    {
        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public void Scenario(ScenarioContext scenario)
        {
            var dbContext = new ApplicationDbContext(scenario.TargetName);

            var level0 = new Branch();
            var level1 = new Branch() { Parent = level0 };
            var level2 = new Branch() { Parent = level1 };

            dbContext.AddRange(level0, level1, level2);
            {
                var result = dbContext.SaveChanges();
                Assert.Equal(3, result);
            }

            scenario.Fact("1. Soft delete works on the deepest level", () => {
                dbContext.Remove(level2);
                var result = dbContext.SaveChanges();

                Assert.Equal(1, result);
                Assert.NotNull(level2.DeletedOn);
            });

            scenario.Fact("2. Soft delete cascades at least 1 level deep", () => {
                dbContext.Remove(level1);
                var result = dbContext.SaveChanges();

                Assert.Equal(2, result);
                Assert.NotNull(level1.DeletedOn);
                Assert.NotNull(level2.DeletedOn);
            });

            scenario.Fact("3. Soft delete cascades multiple levels deep", () => {
                dbContext.Remove(level0);
                var result = dbContext.SaveChanges();

                Assert.Equal(3, result);
                Assert.NotNull(level0.DeletedOn);
                Assert.NotNull(level1.DeletedOn);
                Assert.NotNull(level2.DeletedOn);
            });
        }
    }
}
