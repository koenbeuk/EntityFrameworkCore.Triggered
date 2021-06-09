using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace StudentManager.Triggers.Students
{
    public class AssignRegistrationDate : IBeforeSaveTrigger<Student>
    {
        public Task BeforeSave(ITriggerContext<Student> context, CancellationToken cancellationToken)
        {
            context.Entity.RegistrationDate = DateTime.Today;

            return Task.CompletedTask;
        }
    }
}
