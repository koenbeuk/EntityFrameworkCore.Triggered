using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace StudentManager.Triggers
{
    public class StudentAssignRegistrationDate : IBeforeSaveTrigger<Student>
    {
        public Task BeforeSave(ITriggerContext<Student> context, CancellationToken cancellationToken)
        {
            context.Entity.RegistrationDate = DateTime.Today;

            return Task.CompletedTask;
        }
    }
}
