using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Benchmarks.Triggers
{
    public class SetStudentRegistrationDateTrigger : IBeforeSaveTrigger<Student>
    {
        public Task BeforeSave(ITriggerContext<Student> context, CancellationToken cancellationToken)
        {
            context.Entity.RegistrationDate = DateTimeOffset.Now;
            return Task.CompletedTask;
        }
    }
}
