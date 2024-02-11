using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace StudentManager.Triggers.StudentCourses
{
    public class BlockRemovalWhenCourseIsMandatory(ApplicationDbContext applicationContext) : IBeforeSaveAsyncTrigger<StudentCourse>
    {
        readonly ApplicationDbContext _applicationContext = applicationContext;

        public async Task BeforeSaveAsync(ITriggerContext<StudentCourse> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType == ChangeType.Deleted)
            {
                var course = await _applicationContext.Courses.FindAsync([context.Entity.CourseId], cancellationToken);
                if (course.IsMandatory)
                {
                    throw new InvalidOperationException("Course is required");
                }
            }
        }
    }
}
