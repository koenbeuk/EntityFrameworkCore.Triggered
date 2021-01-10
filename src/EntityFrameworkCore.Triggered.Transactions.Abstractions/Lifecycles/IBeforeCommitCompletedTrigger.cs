using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles
{
    public interface IBeforeCommitCompletedTrigger
    {
        Task BeforeCommitCompleted(CancellationToken cancellationToken);
    }
}
