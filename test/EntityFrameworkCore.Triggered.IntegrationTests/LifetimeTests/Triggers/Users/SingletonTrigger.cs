using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Extensions;

namespace EntityFrameworkCore.Triggered.IntegrationTests.LifetimeTests.Triggers.Users
{

    class SingletonTrigger : IBeforeSaveTrigger<object>
    {
        public SingletonTrigger(TriggerLifetimeTestScenario triggerLifetimeTestScenario)
        {
            triggerLifetimeTestScenario.SingletonTriggerInstances++;
        }

        public Task BeforeSave(ITriggerContext<object> context, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
