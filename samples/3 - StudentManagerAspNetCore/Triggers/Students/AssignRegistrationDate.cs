using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace StudentManager.Triggers.Students
{
    public class AssignRegistrationDate : IBeforeSaveTrigger<Student>
    {
        public void BeforeSave(ITriggerContext<Student> context)
        {
            context.Entity.RegistrationDate = DateTime.Today;
        }
    }
}
