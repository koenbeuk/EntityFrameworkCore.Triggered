namespace EntityFrameworkCore.Triggered.IntegrationTests.LifetimeTests.Triggers.Users;

class ScopedTrigger : IBeforeSaveTrigger<object>
{
    public ScopedTrigger(TriggerLifetimeTestScenario triggerLifetimeTestScenario)
    {
        triggerLifetimeTestScenario.ScopedTriggerInstances++;
    }

    public void BeforeSave(ITriggerContext<object> context) { }
}
