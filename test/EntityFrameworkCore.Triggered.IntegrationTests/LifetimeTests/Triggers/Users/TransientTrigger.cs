using System.Threading;
using System.Threading.Tasks;

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
