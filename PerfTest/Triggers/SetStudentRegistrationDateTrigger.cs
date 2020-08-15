using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace PerfTest.Triggers
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
