namespace EntityFrameworkCore.Triggered.Benchmarks.Triggers;

public class SetStudentRegistrationDateTrigger : IBeforeSaveTrigger<Student>
{
    public void BeforeSave(ITriggerContext<Student> context) => context.Entity.RegistrationDate = DateTimeOffset.Now;
}
