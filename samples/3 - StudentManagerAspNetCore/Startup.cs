using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentManager.Services;

namespace StudentManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddSingleton<EmailService>();

            services
                .AddDbContext<ApplicationDbContext>(options => {
                    options
                        .UseSqlite("Data source=test.db")
                        .UseTriggers(triggerOptions => {
                            triggerOptions.AddTrigger<Triggers.Traits.Audited.CreateAuditRecord>();
                            triggerOptions.AddTrigger<Triggers.Traits.SoftDelete.EnsureSoftDelete>();
                            triggerOptions.AddTrigger<Triggers.Courses.AutoSignupStudents>();
                            triggerOptions.AddTrigger<Triggers.StudentCourses.BlockRemovalWhenCourseIsMandatory>();
                            triggerOptions.AddTrigger<Triggers.StudentCourses.SendWelcomingEmail>();
                            triggerOptions.AddTrigger<Triggers.Students.AssignRegistrationDate>();
                            triggerOptions.AddTrigger<Triggers.Students.SignupToMandatoryCourses>();
                        });

                    options.EnableSensitiveDataLogging(true);
                })
                .AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
            });
        }
    }
}
