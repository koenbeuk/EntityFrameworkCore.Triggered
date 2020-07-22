# EntityFrameworkCore.Triggered 👿
Triggers for EF Core. Respond to changes in your ApplicationDbContext before and after they are committed to the database

![.NET Core](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/workflows/.NET%20Core/badge.svg)

## Getting started
1. Install the package from [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Triggered)
2. Make your DbContext extend from `TriggeredDbContext` (Or read on to learn alternative ways of getting started)
3. Implement Triggers by implementing `IBeforeSaveTrigger<TEntity>` and `IAfterSaveTrigger<TEntity>`
4. View our [samples](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/tree/master/samples)

### Most basic example (without DI)
```csharp
class BeforeSaveStudentTrigger : IBeforeSaveTrigger<Student>
{
    public Task BeforeSave(ITriggerContext<Student> context, CancellationToken cancellationToken)
    {
        if (context.Type == ChangeType.Added){
            context.Entity.RegistrationDate = DateTimeOffset.Today;
        }
        else if (contexType == ChangeType.Modified) {
            if (context.Entity.Entity.Name != context.UnmodifiedEntity.Name) {
                context.Entity.PreviousName = context.Entity.UnmodifiedEntity.Name;
            }
        }

        return Task.CompletedTask;
    }
}

public class Student {
    public int Id { get; set; }
    public string Name { get; set; }
    public string PreviousName { get; set; }
    public DateTimeOffset RegistrationDate { get; set; }
}

public class ApplicationDbContext : TriggeredDbContext {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseTriggers(triggerOptions => {
                triggerOptions.AddTrigger<BeforeSaveStudentTrigger>();
            });

        base.OnConfiguring(optionsBuilder);
    }

	public DbSet<Student> Students { get; set; }
}
```

### ASP.NET Core (and DI)
```csharp
class StudentTrigger  : IBeforeSaveTrigger<Student>, IAfterSaveTrigger<Student>
{
    readonly IEmailService _emailService;
    public StudentTrigger (IEmailService emailservice) {
        _emailService = emailService;
    }

    public Task BeforeSave(ITriggerContext<Student> context, CancellationToken cancellationToken)
    {
        if (_emailService.IsValidEmailAddress(contex.Entity.Email)) { throw new InvalidArgumentException("User email is invalid"); }
        return Task.CompletedTask;
    }

    public async Task AfterSave(ITriggerContext<Student> context, CancellationToken cancellationToken)
    {
        if (context.Type == ChangeType.Added) {
            await _emailService.Send(context.Enity.Email, "Welcome!");
        }
    }
}

public class Student {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set;}
}

public class ApplicationDbContext : TriggeredDbContext {
	public DbSet<Student> Students { get; set; }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EmailService>();
        services
            .AddDbContext<ApplicationContext>()
            .AddScoped<IBeforeSaveTrigger<Course>, BeforeSaveStudentTrigger>()
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    { ... }
}
```

### When you can't inherit from TriggeredDbContext
```csharp
public class ApplicationDbContext : TriggeredDbContext {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseTriggers(triggerOptions => {
                triggerOptions.AddTrigger<BeforeSaveStudentTrigger>();
            });

        base.OnConfiguring(optionsBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var triggerService = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");

        var triggerSession = triggerService.CreateSession(this);

        triggerSession.RaiseBeforeSaveTriggers(default).GetAwaiter().GetResult();
        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        triggerSession.RaiseAfterSaveTriggers(default).GetAwaiter().GetResult();

        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var triggerService = this.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured");

        var triggerSession = triggerService.CreateSession(this);

        await triggerSession.RaiseBeforeSaveTriggers(default);
        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        await triggerSession.RaiseAfterSaveTriggers(default);

        return result;
    }
}
```



### Recursion
`BeforeSaveTrigger<TEntity>` supports basic recursion. This is useful since it allows your triggers to modify for applicationDbContext further and have it call additional triggers. By default this behavior is turned on and protected from infite loops by limiting the number of recursion runs. If you don't like this behavior or want to change it, you can do so by:
```csharp
optionsBuilder.UseTriggers(triggerOptions => {
    triggerOptions.RecursionMode(RecursionMode.EntityAndType).MaxRecusion(20)
})
```

### Inheritence
Triggers support inheritence and sort execution of these triggers based on least concrete to most concrete. Given the following example:
```csharp
interface IAnimal { }
class Animal : IAnimal { }
interface ICat : IAnimal { }
class Cat : Animal, ICat { }
```

Triggers will be executed in that order: First those for `IAnimal`, then those for `Animal`, then those for `ICat` and finally `Cat` itself. If multiple triggers are registered for the same type then they will execute in order or registration with the DI container.

### Priorities
In addition to inheritence and the order in which triggers are registered, a trigger can also implement the `ITriggerPriority` interface. This allows a trigger to configure a custom priority (default: 0). Triggers will then be executed in order of their priority (lower goes first). This means that a trigger for Cat can execute before a trigger for Animal, for as long as its priority is set to run earlier. A convenient set of priorities are exposed in the `CommonTriggerPriority` class
