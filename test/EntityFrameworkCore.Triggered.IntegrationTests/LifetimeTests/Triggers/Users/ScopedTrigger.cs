using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Extensions;

namespace EntityFrameworkCore.Triggered.IntegrationTests.LifetimeTests.Triggers.Users
{
    class ScopedTrigger : IBeforeSaveTrigger<object>
    {
        public ScopedTrigger(TriggerLifetimeTestScenario triggerLifetimeTestScenario)
        {
            triggerLifetimeTestScenario.ScopedTriggerInstances++;
        }

        public Task BeforeSave(ITriggerContext<object> context, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
