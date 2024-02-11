using BlazorTests;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<EventAggregator>();

builder.Services.AddTriggeredDbContextFactory<ApplicationDbContext>(options => {
    options
        .UseSqlite("Data source=test.db")
        .UseTriggers(triggerOptions => {
            triggerOptions.AddAssemblyTriggers();
        });
});

var app = builder.Build();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var contextFactory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    using var context = contextFactory.CreateDbContext();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();