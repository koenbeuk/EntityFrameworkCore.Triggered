namespace EntityFrameworkCore.Triggered.Extensions.Tests;

public abstract class AbstractTrigger : IBeforeSaveTrigger<object>
{
    public void BeforeSave(ITriggerContext<object> context) => throw new NotImplementedException();
}
