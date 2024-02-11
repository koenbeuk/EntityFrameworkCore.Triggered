using EntityFrameworkCore.Triggered;

namespace PrimarySchool.Triggers
{
    public class StudentSignupToMandatoryCourses(ApplicationDbContext applicationContext) : IBeforeSaveTrigger<Student>
    {
        readonly ApplicationDbContext _applicationContext = applicationContext;

        public void BeforeSave(ITriggerContext<Student> context)
        {
            var mandatoryCourses = _applicationContext.Courses
                .Where(x => x.IsMandatory)
                .ToList();

            foreach (var mandatoryCourse in mandatoryCourses)
            {
                var studentRegistration = _applicationContext.StudentCourses.Find(context.Entity.Id, mandatoryCourse.Id);
                if (studentRegistration == null)
                {
                    _applicationContext.StudentCourses.Add(new StudentCourse { StudentId = context.Entity.Id, CourseId = mandatoryCourse.Id });
                }
            }
        }
    }
}
