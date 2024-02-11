using EntityFrameworkCore.Triggered;
using StudentManager.Services;

namespace StudentManager.Triggers.StudentCourses;

public class SendWelcomingEmail(ApplicationDbContext applicationContext, EmailService emailService) : IAfterSaveAsyncTrigger<StudentCourse>
{
    readonly ApplicationDbContext _applicationContext = applicationContext;
    readonly EmailService _emailService = emailService;

    public async Task AfterSaveAsync(ITriggerContext<StudentCourse> context, CancellationToken cancellationToken)
    {
        var student = await _applicationContext.Students.FindAsync([context.Entity.StudentId], cancellationToken);
        var course = await _applicationContext.Courses.FindAsync([context.Entity.CourseId], cancellationToken);

        _emailService.SendEmail(student, $"Welcoming {student.DisplayName} to the course: {course.DisplayName}");
    }
}
