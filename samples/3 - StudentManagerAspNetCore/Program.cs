using Microsoft.EntityFrameworkCore;
using StudentManager;
using StudentManager.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<EmailService>();

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options
        .UseSqlite("Data source=test.db")
        .UseTriggers(triggerOptions => {
            triggerOptions.AddTrigger<StudentManager.Triggers.Traits.Audited.CreateAuditRecord>();
            triggerOptions.AddTrigger<StudentManager.Triggers.Traits.SoftDelete.EnsureSoftDelete>();
            triggerOptions.AddTrigger<StudentManager.Triggers.Courses.AutoSignupStudents>();
            triggerOptions.AddTrigger<StudentManager.Triggers.StudentCourses.BlockRemovalWhenCourseIsMandatory>();
            triggerOptions.AddTrigger<StudentManager.Triggers.StudentCourses.SendWelcomingEmail>();
            triggerOptions.AddTrigger<StudentManager.Triggers.Students.AssignRegistrationDate>();
            triggerOptions.AddTrigger<StudentManager.Triggers.Students.SignupToMandatoryCourses>();

            options.EnableSensitiveDataLogging();
        });
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.Start();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
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