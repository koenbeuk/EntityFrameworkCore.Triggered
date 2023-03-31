# EntityFrameworkCore.Triggered 👿
Triggers for EF Core. Respond to changes in your DbContext before and after they are committed to the database.

[![NuGet version (EntityFrameworkCore.Triggered)](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered/)
[![Build status](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/workflows/.NET%20Core/badge.svg)](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/actions?query=workflow%3A%22.NET+Core%22)

## NuGet packages
- EntityFrameworkCore.Triggered [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered/) [![NuGet](https://img.shields.io/nuget/dt/EntityFrameworkCore.Triggered.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered/)
- EntityFrameworkCore.Triggered.Abstractions [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Abstractions/) [![NuGet](https://img.shields.io/nuget/dt/EntityFrameworkCore.Triggered.Abstractions.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Abstractions/)
- EntityFrameworkCore.Triggered.Transactions [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.Transactions.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Transactions/) [![NuGet](https://img.shields.io/nuget/dt/EntityFrameworkCore.Triggered.Transactions.svg?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Transactions/)
- EntityFrameworkCore.Triggered.Transactions.Abstractions [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.Transactions.Abstractions?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Transactions.Abstractions/) [![NuGet](https://img.shields.io/nuget/dt/EntityFrameworkCore.Triggered.Transactions.Abstractions?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Transactions.Abstractions/)
- EntityFrameworkCore.Triggered.Extensions [![NuGet version](https://img.shields.io/nuget/v/EntityFrameworkCore.Triggered.Extensions?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Extensions/) [![NuGet](https://img.shields.io/nuget/dt/EntityFrameworkCore.Triggered.Extensions?style=flat-square)](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Extensions/)

## Getting started
1. Install the package from [NuGet](https://www.nuget.org/packages/EntityFrameworkCore.Triggered)
2. Write triggers by implementing `IBeforeSaveTrigger<TEntity>` and `IAfterSaveTrigger<TEntity>`
3. Register your triggers with your DbContext
4. View our [samples](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/tree/master/samples) and [more samples](https://github.com/koenbeuk/EntityFrameworkCore.Triggered.Samples) and [a sample application](https://github.com/koenbeuk/EntityFrameworkCore.BookStoreSampleApp)
5. Check out our [wiki](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/wiki) for tips and tricks on getting started and being successful.

### Example
```csharp
class StudentSignupTrigger  : IBeforeSaveTrigger<Student> {
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
    readonly ApplicationDbContext _applicationDbContext;
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

public class ApplicationDbContext : DbContext {
    public DbSet<Student> Students { get; set; }
    public DbSet<Email> Emails { get; set; }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<EmailService>();
        services
            .AddDbContext<ApplicationContext>(options => {
                options.UseTriggers(triggerOptions => {
                    triggerOptions.AddTrigger<StudentSignupTrigger>();
                    triggerOptions.AddTrigger<SendEmailTrigger>();
                });
            })
            .AddTransient<IEmailService, MyEmailService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    { ... }
}
```

### Related articles
[Triggers for Entity Framework Core](https://onthedrift.com/posts/efcore-triggered-part1/) - Introduces the idea of using EF Core triggers in your codebase

[Youtube presentation](https://youtu.be/Gjys0Yebobk?t=671) - Interview by the EF Core team

### Trigger discovery
In the given example, we register triggers directly with our DbContext. This is the recommended approach starting from version 2.3 and 1.4 respectively. If you're on an older version then it's recommended to register triggers with your application's DI container instead:

```csharp
    services
        .AddDbContext<ApplicationContext>(options => options.UseTriggers())
        .AddTransient<IBeforeSaveTrigger<User>, MyBeforeSaveTrigger<User>>();
```

Doing so will make sure that your triggers can use other services registered in your DI container.

You can also use functionality in [EntityFrameworkCore.Triggered.Extensions](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Extensions/) which allows you to discover triggers that are part of an assembly: 

```csharp
services.AddDbContext<ApplicationContext>(options => options.UseTriggers(triggerOptions => triggerOptions.AddAssemblyTriggers()));
// or alternatively
services.AddAssemblyTriggers();
```

### DbContext pooling
When using EF Core's [DbContext pooling](https://docs.microsoft.com/en-us/ef/core/performance/advanced-performance-topics?tabs=with-constant#dbcontext-pooling), Triggers internally needs to discover the `IServiceProvider` that was used to obtain a lease on the current DbContext. Thankfully, all that's complexity is hidden and all that's required is a call to `AddTriggeredDbContextPool`.

```csharp
services.AddDbContextPool<ApplicationDbContext>(...); // Before
services.AddTriggeredDbContextPool<ApplicationDbContext>(...); // After
```

### Cascading changes (previously called Recursion)
`BeforeSaveTrigger<TEntity>` supports cascading triggers. This is useful since it allows your triggers to subsequently modify the same DbContext entity graph and have it raise additional triggers. By default this behavior is turned on and protected from infinite loops by limiting the number of cascading cycles. If you don't like this behavior or want to change it, you can do so by:
```csharp
optionsBuilder.UseTriggers(triggerOptions => {
    triggerOptions.CascadeBehavior(CascadeBehavior.EntityAndType).MaxRecusion(20)
})
```

Currently there are 2 types of cascading strategies out of the box: `NoCascade` and `EntityAndType` (default). The former simply disables cascading, whereas the latter cascades triggers for as long as the combination of the Entity and the change type is unique. `EntityAndType` is the recommended and default cascading strategy. You can also provide your own implemention.

### Inheritance
Triggers support inheritance and sort execution of these triggers based on least concrete to most concrete. Given the following example:
```csharp
interface IAnimal { }
class Animal : IAnimal { }
interface ICat : IAnimal { }
class Cat : Animal, ICat { }
```

In this example, triggers will be executed in the order: 
* those for `IAnimal`, 
* those for `Animal`
* those for `ICat`, and finally 
* `Cat` itself. 

If multiple triggers are registered for the same type, they will execute in order they were registered with the DI container.

### Priorities
In addition to inheritance and the order in which triggers are registered, a trigger can also implement the `ITriggerPriority` interface. This allows a trigger to configure a custom priority (default: 0). Triggers will then be executed in order of their priority (lower goes first). This means that a trigger for `Cat` can execute before a trigger for `Animal`, for as long as its priority is set to run earlier. A convenient set of priorities are exposed in the `CommonTriggerPriority` class.

### Error handling
In some cases, you want to be triggered when a `DbUpdateException` occurs. For this purpose we have `IAfterSaveFailedTrigger<TEntity>`. This gets triggered for all entities as part of the change set when DbContext.SaveChanges raises a DbUpdateException. The handling method: `AfterSaveFailed` in turn gets called with the trigger context containing the entity as well as the exception. You may attempt to call `DbContext.SaveChanges` again from within this trigger. This will not raise triggers that are already raised and only raise triggers that have since become relevant (based on the cascading configuration). 

### Lifecycle triggers
Starting with version 2.1.0, we added support for "Lifecycle triggers". These triggers are invoked once per trigger type per `SaveChanges` lifecyle and reside within the   `EntityFrameworkCore.Triggered.Lifecycles` namespace. These can be used to run something before/after all individual triggers have run. Consider the following example:
```csharp
public BulkReportTrigger : IAfterSaveTrigger<Email>, IAfterSaveCompletedTrigger {
    private List<string> _emailAddresses = new List<string>();
    
    // This may be invoked multiple times within the same SaveChanges call if there are multiple emails
    public Task AfterSave(ITriggerContext<Email> context, CancellationToken cancellationToken) { 
        if (context.ChangeType == ChangeType.Added) { 
            this._emailAddresses.Add(context.Address);
            return Task.CompletedTask;
        }
    }
    
    public Task AfterSaveCompleted(CancellationToken cancellationToken) {
        Console.WriteLine($"We've sent {_emailAddresses.Count()} emails to {_emailAddresses.Distinct().Count()}" distinct email addresses");
        return Task.CompletedTask;
    }
}
```

### Transactions
Many database providers support the concept of a Transaction. By default when using SqlServer with EntityFrameworkCore, any call to `SaveChanges` will be wrapped in a transaction. Any changes made in `IBeforeSaveTrigger<TEntity>` will be included within the transaction and changes made in `IAfterSaveTrigger<TEntity>` will not. However, it is possible for the user to [explicitly control transactions](https://docs.microsoft.com/en-us/ef/core/saving/transactions). Triggers are extensible and one such extension are [Transactional Triggers](https://www.nuget.org/packages/EntityFrameworkCore.Triggered.Transactions/). In order to use this plugin you will have to implement a few steps:
```csharp
// OPTIONAL: Enable transactions when configuring triggers (Required ONLY when not using dependency injection)
triggerOptions.UseTransactionTriggers();
...
using var tx = context.Database.BeginTransaction();
var triggerService = context.GetService<ITriggerService>(); // ITriggerService is responsible for creating now trigger sessions (see below)
var triggerSession = triggerService.CreateSession(context); // A trigger session keeps track of all changes that are relevant within that session. e.g. RaiseAfterSaveTriggers will only raise triggers on changes it discovered within this session (through RaiseBeforeSaveTriggers)

try {
    await context.SaveChangesAsync();
    await triggerSession.RaiseBeforeCommitTriggers();    
    await context.CommitAsync();
    await triggerSession.RaiseAfterCommitTriggers();
}
catch {
    await triggerSession.RaiseBeforeRollbackTriggers();
    await context.RollbackAsync();
    await triggerSession.RaiseAfterRollbackTriggers();	
    throw;
}
```
In this example we were not able to inherit from TriggeredDbContext since we want to manually control the TriggerSession

### Custom trigger types
By default we offer 3 trigger types: `IBeforeSaveTrigger`, `IAfterSaveTrigger` and `IAfterSaveFailedTrigger`. These will cover most cases. In addition we offer `IRaiseBeforeCommitTrigger` and `IRaiseAfterCommitTrigger` as an extension to further enhance your control of when triggers should run. We also offer support for custom triggers. Let's say we want to react to specific events happening in your context. We can do so by creating a new interface `IThisThingJustHappenedTrigger` and implementing an extension method for `ITriggerSession` to invoke triggers of that type. Please take a look at how [Transactional triggers](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/tree/master/src/EntityFrameworkCore.Triggered.Transactions) are implemented as an example.

### Async triggers
Async triggers are fully supported, though you should be aware that if they are fired as a result of a call to the synchronous `SaveChanges` on your DbContext, the triggers will be invoked and the results waited for by blocking the caller thread as discussed [here](https://github.com/koenbeuk/EntityFrameworkCore.Triggered/issues/127). This is known as the sync-over-async problem which can result in deadlocks. It's recommended to use `SaveChangesAsync` to avoid the potential for deadlocks, which is also best practice anyway for an operation that involves network/file access as is the case with an EF Core read/write to the database.

> Starting from v4 (currently in preview), we offer both synchronous and asynchronous triggers.

### Versions
- [V4-preview](https://www.nuget.org/packages/EntityFrameworkCore.Triggered/4.0.0) is an evolution of this library with breaking changes. This is still in production and not ready for production use.
- [V3](https://www.nuget.org/packages/EntityFrameworkCore.Triggered/3.2.2) is available for those targeting EF Core 6 or later and is the current stable offering.
- [V2](https://www.nuget.org/packages/EntityFrameworkCore.Triggered/1.4.0) Targets EF Core 3.1. This requires you to inherit from `TriggeredDbContext` or handle manual management of trigger sessions.
