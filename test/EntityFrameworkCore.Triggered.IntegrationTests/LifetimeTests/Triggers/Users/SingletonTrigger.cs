using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.IntegrationTests.LifetimeTests.Triggers.Users
{

    class SingletonTrigger : IBeforeSaveTrigger<object>
    {
        public SingletonTrigger(TriggerLifetimeTestScenario triggerLifetimeTestScenario)
        {
            triggerLifetimeTestScenario.SingletonTriggerInstances++;
        }

        public void BeforeSave(ITriggerContext<object> context) { }
    }
}
