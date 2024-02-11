using System.Linq;

namespace EntityFrameworkCore.Triggered.Benchmarks.Triggers
{
    public class SignStudentUpForMandatoryCourses(TriggeredApplicationContext applicationContext) : IBeforeSaveTrigger<Student>
    {
        readonly TriggeredApplicationContext _applicationContext = applicationContext;

        public void BeforeSave(ITriggerContext<Student> context)
        {
            var mandatoryCourses = _applicationContext.Courses.Where(x => x.IsMandatory).ToList();

            foreach (var mandatoryCourse in mandatoryCourses)
            {
                _applicationContext.StudentCourses.Add(new StudentCourse {
                    CourseId = mandatoryCourse.Id,
                    StudentId = context.Entity.Id
                });
            }
        }
    }
}
