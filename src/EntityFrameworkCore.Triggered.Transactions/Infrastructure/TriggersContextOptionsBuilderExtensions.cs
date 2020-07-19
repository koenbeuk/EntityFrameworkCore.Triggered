using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Infrastructure;

namespace EntityFrameworkCore.Triggered.Transactions.Infrastructure
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
