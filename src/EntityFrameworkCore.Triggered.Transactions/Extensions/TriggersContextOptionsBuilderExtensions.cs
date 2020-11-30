using System;
using EntityFrameworkCore.Triggered.Infrastructure;
using EntityFrameworkCore.Triggered.Transactions;

namespace Microsoft.EntityFrameworkCore
{
    public static class TriggersContextOptionsBuilderExtensions
    {
        public static TriggersContextOptionsBuilder UseTransactionTriggers(this TriggersContextOptionsBuilder triggersContextOptionsBuilder)
        {
            if (triggersContextOptionsBuilder is null)
            {
                throw new ArgumentNullException(nameof(triggersContextOptionsBuilder));
            }

            return triggersContextOptionsBuilder
                .AddTriggerType(typeof(IBeforeCommitTrigger<>))
                .AddTriggerType(typeof(IAfterCommitTrigger<>))
                .AddTriggerType(typeof(IBeforeRollbackTrigger<>))
                .AddTriggerType(typeof(IAfterRollbackTrigger<>));
        }
    }
}
