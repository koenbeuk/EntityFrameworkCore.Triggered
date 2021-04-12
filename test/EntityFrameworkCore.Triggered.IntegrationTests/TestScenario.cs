using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.IntegrationTests;
using EntityFrameworkCore.Triggered.IntegrationTests.Models;
using Linker.ScenarioTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public partial class TestScenario
    {
        [Scenario]
        public void TestScenario1(ScenarioContext scenario)
        {
            const int sampleUsersCount = 100;

            var dbContext = new ApplicationDbContext();

            // step 1: Populate database with users
            {
                dbContext.Users.AddRange(Enumerable.Range(0, sampleUsersCount).Select(x => new User {
                    UserName = $"user{x}"
                }));
             
                dbContext.SaveChanges();
            }

            scenario.Fact("Database saved all users", () => {
                var usersCount = dbContext.Users.Count();
                Assert.Equal(sampleUsersCount, usersCount);
            });
        }
    }
}
