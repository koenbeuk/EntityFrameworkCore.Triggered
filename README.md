# EntityFrameworkCore.Triggered
Triggers for EF Core. Respond to changes in your ApplicationDbContext before and after they are committed to the database

## Getting started
1. Install the package from [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Triggered)
2. Make your DbContext extend from `TriggeredDbContext` (Or read on to learn alternative ways of getting started)
3. Implement Triggers by implementing `IBeforeSaveTrigger<TEntity>` and `IAfterSaveTrigger<TEntity>`
4. View our [samples]](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/tree/master/samples)

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
                context.Entity.Name = context.Entity.UnmodifiedEntity.Name;
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

### Configuring recursion
`BeforeSaveTrigger<TEntity>` supports basic recursion. This is useful since it allows your triggers to modify for applicationDbContext further and have it call additional triggers. By default this behavior is turned on and protected from infite loops by limiting the number of recursion runs. If you don't like this behavior or want to change it, you can do so by:
```csharp
optionsBuilder.UseTriggers(triggerOptions => {
    triggerOptions.RecursionMode(RecursionMode.EntityAndType).MaxRecusion(20)
})
```

### Roadmap
1. Support for triggering before and after transactions commit
2. Extensibility model for your own custom trigger types
