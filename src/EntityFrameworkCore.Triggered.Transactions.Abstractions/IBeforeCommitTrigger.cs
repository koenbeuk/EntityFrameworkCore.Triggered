using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IBeforeCommitTrigger<in TEntity>
        where TEntity : class
    {
        Task BeforeCommit(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
