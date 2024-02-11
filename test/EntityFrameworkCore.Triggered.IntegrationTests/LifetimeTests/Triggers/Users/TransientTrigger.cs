namespace EntityFrameworkCore.Triggered.IntegrationTests.LifetimeTests.Triggers.Users
{
    class TransientTrigger : IBeforeSaveTrigger<object>
    {
        public TransientTrigger(TriggerLifetimeTestScenario triggerLifetimeTestScenario)
        {
            triggerLifetimeTestScenario.TransientTriggerInstances++;
        }

        public void BeforeSave(ITriggerContext<object> context) { }
    }
}
