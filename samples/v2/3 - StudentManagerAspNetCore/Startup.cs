using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
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
                .AddTriggeredDbContextPool<ApplicationContext>(options => {
                    options
                        .UseSqlite("Data source=test.db");
                })
                .AddHttpContextAccessor()
                .AddScoped<IBeforeSaveTrigger<Traits.ISoftDelete>, Triggers.Traits.SoftDelete.EnsureSoftDelete>()
                .AddScoped<IBeforeSaveTrigger<Traits.IAudited>, Triggers.Traits.Audited.CreateAuditRecord>()

                .AddScoped<IBeforeSaveTrigger<Course>, Triggers.Courses.AutoSignupStudents>()
                .AddScoped<IBeforeSaveTrigger<Student>, Triggers.Students.AssignRegistrationDate>()
                .AddScoped<IBeforeSaveTrigger<Student>, Triggers.Students.SignupToMandatoryCourses>()
                .AddScoped<IBeforeSaveTrigger<StudentCourse>, Triggers.StudentCourses.BlockRemovalWhenCourseIsMandatory>()
                .AddScoped<IAfterSaveTrigger<StudentCourse>, Triggers.StudentCourses.SendWelcomingEmail>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
