using System.Linq;
using EntityFrameworkCore.Triggered.IntegrationTests.SampleStore.Models;
using ScenarioTests;
using Xunit;

namespace EntityFrameworkCore.Triggered.IntegrationTests.SampleStore
{
    public partial class TestScenario
    {
        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public void TestScenario1(ScenarioContext scenario)
        {
            const int sampleUsersCount = 100;

            var dbContext = new ApplicationDbContext(scenario.TargetName);

            // step 1: Populate database with users
            dbContext.Users.AddRange(Enumerable.Range(0, sampleUsersCount).Select(x => new User {
                UserName = $"user{x}"
            }));

            dbContext.SaveChanges();

            scenario.Fact("Database saved all users", () => {
                var usersCount = dbContext.Users.Count();
                Assert.Equal(sampleUsersCount, usersCount);
            });

            // step 2: Delete the last 50 users
            dbContext.RemoveRange(
                dbContext.Users.Where(x => x.Id > sampleUsersCount / 2)
            );

            dbContext.SaveChanges();

            scenario.Fact("We should still have all our users", () => {
                var usersCount = dbContext.Users.Count();
                Assert.Equal(sampleUsersCount, usersCount);
            });

            scenario.Fact("However, half of them  will have been soft deleted", () => {
                var usersCount = dbContext.Users.Where(x => x.DeletedDate != null).Count();
                Assert.Equal(sampleUsersCount / 2, usersCount);
            });

            // step 3: Undo deletion of our users
            {
                var users = dbContext.Users.Where(x => x.DeletedDate != null);
                foreach (var user in users)
                {
                    user.DeletedDate = null;
                }

                dbContext.SaveChanges();
            }

            scenario.Fact("Ensure that all users are restored", () => {
                var usersCount = dbContext.Users.Where(x => x.DeletedDate != null).Count();
                Assert.Equal(0, usersCount);
            });
        }
    }
}
