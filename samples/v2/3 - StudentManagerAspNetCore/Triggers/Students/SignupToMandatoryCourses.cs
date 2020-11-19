using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;

namespace StudentManager.Triggers.Students
{
    public class SignupToMandatoryCourses : IBeforeSaveTrigger<Student>
    {
        readonly ApplicationDbContext _applicationContext;

        public SignupToMandatoryCourses(ApplicationDbContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task BeforeSave(ITriggerContext<Student> context, CancellationToken cancellationToken)
        {  
            var mandatoryCourses = await _applicationContext.Courses
                .Where(x => x.IsMandatory)
                .ToListAsync(cancellationToken);
            
            foreach (var mandatoryCourse in mandatoryCourses)
            {
                var studentRegistration = _applicationContext.StudentCourses.Find(context.Entity.Id, mandatoryCourse.Id);
                if (studentRegistration == null)
                {
                    _applicationContext.StudentCourses.Add(new StudentCourse { Student = context.Entity, Course = mandatoryCourse });
                }
            }
        }
    }
}
