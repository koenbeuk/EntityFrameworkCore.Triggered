using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;

namespace EntityFrameworkCore.Triggered.Transactions.Internal
{
    public class AfterRollbackTriggerAdapter : TriggerAdapterBase
    {
        public AfterRollbackTriggerAdapter(object trigger) : base(trigger)
        {
        }

        public override Task Execute(object triggerContext, CancellationToken cancellationToken)
            => Execute(nameof(IAfterRollbackTrigger<object>.AfterRollback), triggerContext, cancellationToken);
    }
}
