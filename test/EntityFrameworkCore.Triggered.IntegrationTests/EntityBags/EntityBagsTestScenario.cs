using Microsoft.EntityFrameworkCore;
using ScenarioTests;
using Xunit;

namespace EntityFrameworkCore.Triggered.IntegrationTests.EntityBags;

public partial class EntityBagsTestScenario
{
    [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
    public void PlayScenario(ScenarioContext scenario)
    {
        using var dbcontext = new ApplicationDbContext(
            new DbContextOptionsBuilder()
                .UseInMemoryDatabase(scenario.TargetName)
                .UseTriggers(triggerOptions => {
                    triggerOptions.AddTrigger<Triggers.StampModifiedOnTrigger>();
                    triggerOptions.AddTrigger<Triggers.SoftDeleteTrigger>();
                })
                .Options
        );

        var user = new User();
        dbcontext.Users.Add(user);
        dbcontext.SaveChanges();

        scenario.Fact("ModifiedOn is null", () => Assert.Null(user.ModifiedOn));
        scenario.Fact("DeletedOn is null", () => Assert.Null(user.DeletedOn));

        dbcontext.Remove(user);
        dbcontext.SaveChanges();

        scenario.Fact("ModifiedOn is null after removal", () => Assert.Null(user.ModifiedOn));
        scenario.Fact("DeletedOn is not null after removal", () => Assert.NotNull(user.DeletedOn));

        user.Name = "Jon";
        dbcontext.SaveChanges();

        scenario.Fact("ModifiedOn is not null after update", () => Assert.NotNull(user.ModifiedOn));
    }
}
