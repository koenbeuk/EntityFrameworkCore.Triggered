using EntityFrameworkCore.Triggered.Infrastructure;
using EntityFrameworkCore.Triggered.Transactions;
using EntityFrameworkCore.Triggered.Transactions.Lifecycles;

namespace Microsoft.EntityFrameworkCore;

public static class TriggersContextOptionsBuilderExtensions
{
    public static TriggersContextOptionsBuilder UseTransactionTriggers(this TriggersContextOptionsBuilder triggersContextOptionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(triggersContextOptionsBuilder);

        return triggersContextOptionsBuilder
            .AddTriggerType(typeof(IBeforeCommitTrigger<>))
            .AddTriggerType(typeof(IBeforeCommitAsyncTrigger<>))
            .AddTriggerType(typeof(IAfterCommitTrigger<>))
            .AddTriggerType(typeof(IAfterCommitAsyncTrigger<>))
            .AddTriggerType(typeof(IBeforeRollbackTrigger<>))
            .AddTriggerType(typeof(IBeforeRollbackAsyncTrigger<>))
            .AddTriggerType(typeof(IAfterRollbackTrigger<>))
            .AddTriggerType(typeof(IAfterRollbackAsyncTrigger<>))
            .AddTriggerType(typeof(IBeforeCommitStartingTrigger))
            .AddTriggerType(typeof(IBeforeCommitStartingAsyncTrigger))
            .AddTriggerType(typeof(IBeforeCommitCompletedTrigger))
            .AddTriggerType(typeof(IBeforeCommitCompletedAsyncTrigger))
            .AddTriggerType(typeof(IAfterCommitStartingTrigger))
            .AddTriggerType(typeof(IAfterCommitStartingAsyncTrigger))
            .AddTriggerType(typeof(IAfterCommitCompletedTrigger))
            .AddTriggerType(typeof(IAfterCommitCompletedAsyncTrigger));
    }
}
