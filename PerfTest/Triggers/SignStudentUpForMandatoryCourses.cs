using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace PerfTest.Triggers
{
    public class SignStudentUpForMandatoryCourses : IBeforeSaveTrigger<Student>
    {
        readonly ApplicationContext _applicationContext;

        public SignStudentUpForMandatoryCourses(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public Task BeforeSave(ITriggerContext<Student> context, CancellationToken cancellationToken)
        {
            var mandatoryCourses = _applicationContext.Courses.Where(x => x.IsMandatory).ToList();

            foreach (var mandatoryCourse in mandatoryCourses)
            {
                _applicationContext.StudentCourses.Add(new StudentCourse {
                    CourseId = mandatoryCourse.Id,
                    StudentId = context.Entity.Id
                });
            }

            return Task.CompletedTask;
        }
    }
}
