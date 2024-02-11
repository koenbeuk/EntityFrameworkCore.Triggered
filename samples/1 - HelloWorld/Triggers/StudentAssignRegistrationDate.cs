using System;
using EntityFrameworkCore.Triggered;

namespace PrimarySchool.Triggers
{
    public class StudentAssignRegistrationDate : IBeforeSaveTrigger<Student>
    {
        public void BeforeSave(ITriggerContext<Student> context)
        {
            if (context.ChangeType == ChangeType.Added)
            {
                context.Entity.RegistrationDate = DateTime.Today;
            }
        }
    }
}
