using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScenarioTests;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal;

public partial class HybridServiceProviderTests
{
    class CustomDbContext : DbContext
    {

    }

    class ConcreteCustomDbContext : CustomDbContext
    {

    }

    [Scenario]
    public void ScenarioTests(ScenarioContext scenario)
    {
        var dbContext = new ConcreteCustomDbContext();
        var serviceProvider = new ServiceCollection()
            .AddSingleton("X")
            .BuildServiceProvider();

        var subject = new HybridServiceProvider(serviceProvider, dbContext);

        scenario.Fact("Can retrieve DbContext from ServiceProvider", () => {
            var result = subject.GetService<DbContext>();
            Assert.NotNull(result);
        });

        scenario.Fact("Can retrieve CustomDbContext from ServiceProvider", () => {
            var result = subject.GetService<CustomDbContext>();
            Assert.NotNull(result);
        });

        scenario.Fact("Can retrieve ConcreteCustomDbContext from ServiceProvider", () => {
            var result = subject.GetService<ConcreteCustomDbContext>();
            Assert.NotNull(result);
        });

        scenario.Fact("Can retrieve custom service from ServiceProvider", () => {
            var result = subject.GetService<string>();
            Assert.NotNull(result);
        });
    }

    [Fact]
    public void CantOverrideDbContext()
    {
        var dbContext1 = new ConcreteCustomDbContext();
        var dbContext2 = new ConcreteCustomDbContext();

        var serviceProvider = new ServiceCollection()
            .AddSingleton(dbContext2)
            .BuildServiceProvider();

        var subject = new HybridServiceProvider(serviceProvider, dbContext1);

        var result = subject.GetRequiredService<ConcreteCustomDbContext>();

        Assert.Equal(dbContext1, result);
    }
}
