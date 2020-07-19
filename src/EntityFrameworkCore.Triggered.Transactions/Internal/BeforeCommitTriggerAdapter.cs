using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;

namespace EntityFrameworkCore.Triggered.Transactions.Internal
{
    public class BeforeCommitTriggerAdapter : TriggerAdapterBase
    {
        public BeforeCommitTriggerAdapter(object trigger) : base(trigger)
        {
        }

        public override Task Execute(object triggerContext, CancellationToken cancellationToken)
            => Execute(nameof(IBeforeCommitTrigger<object>.BeforeCommit), triggerContext, cancellationToken);
    }
}
