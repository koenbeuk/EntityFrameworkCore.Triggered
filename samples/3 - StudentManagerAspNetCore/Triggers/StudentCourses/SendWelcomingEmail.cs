using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using StudentManager.Services;

namespace StudentManager.Triggers.StudentCourses
{
    public class SendWelcomingEmail : IAfterSaveTrigger<StudentCourse>
    {
        readonly ApplicationContext _applicationContext;
        readonly EmailService _emailService;

        public SendWelcomingEmail(ApplicationContext applicationContext, EmailService emailService)
        {
            _applicationContext = applicationContext;
            _emailService = emailService;
        }

        public async Task AfterSave(ITriggerContext<StudentCourse> context, CancellationToken cancellationToken)
        {
            var student = await _applicationContext.Students.FindAsync(new object[] { context.Entity.StudentId }, cancellationToken);
            var course = await _applicationContext.Courses.FindAsync(new object[] { context.Entity.CourseId }, cancellationToken);

            _emailService.SendEmail(student, $"Welcoming {student.DisplayName} to the course: {course.DisplayName}");
        }
    }
}
