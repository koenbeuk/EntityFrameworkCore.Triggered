# EntityFrameworkCore.Triggered 👿
Triggers for EF Core. Respond to changes in your ApplicationDbContext before and after they are committed to the database.

[![NuGet version (EntityFrameworkCore.Triggered)](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered/)
[![Build status](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/workflows/.NET%20Core/badge.svg)](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/actions?query=workflow%3A%22.NET+Core%22)

## NuGet packages
- EntityFrameworkCore.Triggered [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered/)
- EntityFrameworkCore.Triggered.Abstractions [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Abstractions/)
- EntityFrameworkCore.Triggered.Transactions [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.Transactions.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Transactions/)
- EntityFrameworkCore.Triggered.Transactions.Abstractions [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.Transactions.Abstractions?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Transactions.Abstractions/)
- EntityFrameworkCore.Triggered.AspNetCore [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.AspNetCore.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.AspNetCore/)

## Getting started
1. Install the package from [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Triggered)
2. Make your DbContext extend from `TriggeredDbContext` (Or read on to learn alternative ways of getting started)
3. Implement Triggers by implementing `IBeforeSaveTrigger<TEntity>` and `IAfterSaveTrigger<TEntity>`
4. View our [samples](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/tree/master/samples)

> With the upcoming release of EFCore 5.0 and its ability to [intercept SaveChanges](https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-5.0/whatsnew#savechanges-interception-and-events), we no longer need to extend from `TriggeredDbContext`. This will be the default behavior going forward and is represented in v2 of this project (currently available as a beta release).

### Example
```csharp
class StudentSignupTrigger  : IBeforeSaveTrigger<Student>, IAfterSaveTrigger<Student> {
    readonly ApplicationDbContext _applicationDbContext;
    
    public class StudentTrigger(ApplicationDbContext applicationDbContext) {
        _applicationDbContext = applicationDbContext;
    }

    public Task BeforeSave(ITriggerContext<Student> context, CancellationToken cancellationToken) {   
        if (context.ChangeType == ChangeType.Added){
            _applicationDbContext.Emails.Add(new Email {
                Student = context.Entity, 
                Title = "Welcome!";,
                Body = "...."
            });
        } 

        return Task.CompletedTask;
    }
}

class SendEmailTrigger : IAfterSaveTrigger<Email> {
    readonly IEmailService _emailService;
    public StudentTrigger (ApplicationDbContext applicationDbContext, IEmailService emailservice) {
        _applicationDbContext = applicationDbContext;
        _emailService = emailService;
    }

    public async Task AfterSave(ITriggerContext<Student> context, CancellationToken cancellationToken) {
        if (context.Entity.SentDate == null && context.ChangeType != ChangeType.Deleted) {
            await _emailService.Send(context.Enity);
            context.Entity.SentDate = DateTime.Now;

            await _applicationContext.SaveChangesAsync();
        }
    }
}

public class Student {
    public int Id { get; set; }
    public string Name { get; set; }
    public string EmailAddress { get; set;}
}

public class Email { 
    public int Id { get; set; }  
    public Student Student { get; set; } 
    public DateTime? SentDate { get; set; }
}

public class ApplicationDbContext : TriggeredDbContext {
    public DbSet<Student> Students { get; set; }
    public DbSet<Email> Emails { get; set; }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EmailService>();
        services
            .AddTriggeredDbContext<ApplicationContext>()
            .AddScoped<IBeforeSaveTrigger<Course>, BeforeSaveStudentTrigger>()
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    { ... }
}
```

### Recursion
`BeforeSaveTrigger<TEntity>` supports recursion. This is useful since it allows your triggers to subsequently modify the same DbContext entity graph and have it raise additional triggers. By default this behavior is turned on and protected from infite loops by limiting the number of recursion runs. If you don't like this behavior or want to change it, you can do so by:
```csharp
optionsBuilder.UseTriggers(triggerOptions => {
    triggerOptions.RecursionMode(RecursionMode.EntityAndType).MaxRecusion(20)
})
```

Currently there are 2 types of recursion strategies out of the box, with the support to providing your own:  `NoRecursion` and `EntityAndTypeRecursion` (default). The former simply disables recursion whereas the latter recursively detects changes and raises triggers for those changes for as long as the combination of the Entity and the change type is uniuqe. `EntityAndTypeRecursion` is the recommended and default recursion strategy.

### Inheritance
Triggers support inheritance and sort execution of these triggers based on least concrete to most concrete. Given the following example:
```csharp
interface IAnimal { }
class Animal : IAnimal { }
interface ICat : IAnimal { }
class Cat : Animal, ICat { }
```

Triggers will be executed in that order: First those for `IAnimal`, then those for `Animal`, then those for `ICat` and finally `Cat` itself. If multiple triggers are registered for the same type then they will execute in order or registration with the DI container.

### Priorities
In addition to inheritance
and the order in which triggers are registered, a trigger can also implement the `ITriggerPriority` interface. This allows a trigger to configure a custom priority (default: 0). Triggers will then be executed in order of their priority (lower goes first). This means that a trigger for Cat can execute before a trigger for Animal, for as long as its priority is set to run earlier. A convenient set of priorities are exposed in the `CommonTriggerPriority` class

### Dependency injection
EntityFrameworkCore.Triggered integrates itself with EntityFrameworkCore as an extension and therefore uses EntityFrameworkCore's internal ServiceCollection to register and resolve internal dependencies. Triggers are then registered with either the same ServiceCollection or one controlled by the application. when the application does not use dependency injection, the default service provider will be EF cores internal service provider. When the context gets resolved through dependency injection, the default service provider wll be a new scope from the application service provider.

When using `optionsBuilder.UseTriggers(triggerOptions => triggerOptions.AddTrigger(...))`, these triggers will get registered with the internal ServiceCollection and therefore have no access to externally declared Services (Including the DbContext itself). This is great for testing or when dependency injection is not relevant however the recommended approach is to register triggers with a user controller ServiceCollection and provice EntityFrameworkCore.Triggered a way of resolving a ServiceProvider. this can be achieved through various ways:

- Use one of our integration packages. Currently we only support direct integration with ASP.NET Core through the [EntityFrameworkCore.Triggered.AspNetCore](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.AspNetCore/) nuget package. This will use the IHttpContextAccessor to access the scoped ServiceProvider of the current request. To use this, please make sure to either call: `services.AddAspNetCoreTriggeredDbContext<MyTriggeredDbContext>()` or `triggerOptions.UseAspNetCoreIntegration()`.
- TriggeredDbContext implements a constructor overload that accepts an IServiceProvider. Your derived class can implement a constructor that accepts a IServiceProvider as well and forward that to the base class constructor. This will make sure that when you use DependencyInjection through `services.AddDbContext<MyTriggeredDbContext>()`, the ServiceProvider that is used to resolve the DbContext will be sent in as an argument for the ServiceProvider. Note that this does not work for pooled contexts since those are shared and essentially behave as a singleton.
- Configure a method that returns the current relevant ServiceProvider. This can be done by providing a method through: `triggerOptions.UseApplicationScopedServiceProviderAccessor(sp => ...)`. The first argument will be either the application ServiceProvider if available or otherwise the internal service provider.

### Error handling
In some cases, you want to be triggered when a DbUpdateException occurs. For this purpose we have `IAfterSaveFailedTrigger<TEntity>`. This gets triggered for all entities as part of the change set when DbContext.SaveChanges raises a DbUpdateException. The handling method: `AfterSaveFailed` in turn gets called with the trigger context containing the entity as well as the exception. You may attempt to call `DbContext.SaveChanges` again from within this trigger. This will not raise triggers that are already raised and only raise triggers that have since become relevant (based on recursive configuration). 

### Transactions
Many database providers support the concept of a Transaction. By default when using SqlServer with EntityFrameworkCore, any call to SaveChanges will be wrapped in a transaction. Any changes made in `IBeforeSaveTrigger<TEntity>` will be included within the transaction and changes made in `IAfterSaveTrigger<TEntity>` will not. However, it is possible for the user to [explicitly control transactions](https://docs.microsoft.com/en-us/ef/core/saving/transactions). Triggers are extensible and one such extension are [Transactional Triggers](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Transactions/). In order to use this plugin you will have to implement a few steps:
```csharp
// OPTIONAL: Enable transactions when configuring triggers (Required ONLY when not using dependency injection)
triggerOptions.UseTransactionTriggers();
...
using var tx = context.Database.BeginTransaction();
var triggerService = context.GetService<ITriggerService>(); // ITriggerService is responsible for creating now trigger sessions (see below)
var triggerSession = triggerService.CreateSession(context); // A trigger session keeps track of all changes that are relevant within that session. e.g. RaiseAfterSaveTriggers will only raise triggers on changes it discovered within this session (through RaiseBeforeSaveTriggers)

await triggerSession.RaiseBeforeSaveTriggers();

try {
	await context.SaveChangesAsync();
	await triggerSession.RaiseAfterSaveTriggers();
}
catch {
	await triggerSession.RaiseBeforeRollbackTriggers();
	await context.RollbackAsync();
	await triggerSession.RaiseAfterRollbackTriggers();	
	throw;
}

await triggerSession.RaiseBeforeCommitTriggers();
await context.CommitAsync();
await triggerSession.RaiseAfterCommitTriggers();
```
In this example we were not able to inherit from TriggeredDbContext since we want to manually control the TriggerSession

### Custom trigger types
By default we offer 3 trigger types: `IBeforeSaveTrigger`, `IAfterSaveTrigger` and `IAfterSaveFailedTrigger`. These will cover most cases. In addition we offer `IRaiseBeforeCommitTrigger` and `IRaiseAfterCommitTrigger` as an extension to further enhance your control of when triggers should run. We also offer support for custom triggers. Lets say we want to react to specific events happening in your context. We can do so by creating a new interface: IThisThingJustHappenedTrigger and implementing an extension method for ITriggerSession to invoke triggers of that type. Please take a look at how [Transactional triggers](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/tree/master/src/EntityFrameworkCore.Triggered.Transactions) are implemented as an example.

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

### When you  don't want to use dependeny injection
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

### Similar products
- [Ramses](https://github.com/JValck/Ramses): Lifecycle hooks for EFCore. A simple yet effective way of reacting to changes. Great for situations where you simply want to make sure that a property is set before saving to the database. Limited though in features as there is no dependency injection, no async support, no extensibility model and lifeycle hooks need to be implemented on the entity type itself.
- [EntityFramework.Triggers](https://github.com/NickStrupat/EntityFramework.Triggers). Add triggers to your entities with insert, update, and delete events. There are three events for each: before, after, and upon failure. A fine alternative to EntityFrameworkCore.Triggered. It has been around for some time and has support for EF6 and boast a decent community. There are plenty of trigger types to opt into including the option to cancel SaveChanges from within a trigger. A big drawback however is that it does not support recursion so that triggers can never be relied on to enforce a domain constraint.
