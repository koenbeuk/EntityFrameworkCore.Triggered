using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace StudentManager.Triggers
{
    public class BockStudentCourseRemovalWhenCourseIsMandatory : IBeforeSaveTrigger<StudentCourse>
    {
        readonly ApplicationContext _applicationContext;

        public BockStudentCourseRemovalWhenCourseIsMandatory(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task BeforeSave(ITriggerContext<StudentCourse> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Deleted)
            {
                var course = await _applicationContext.Courses.FindAsync(new object[] { context.Entity.CourseId }, cancellationToken);
                if (course.IsMandatory)
                {
                    throw new InvalidOperationException("Course is required");
                }
            }
        }
    }
}
