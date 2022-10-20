using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles
{
    public interface IAfterCommitCompletedAsyncTrigger
    {
        Task AfterCommitCompletedAsync(CancellationToken cancellationToken);
    }
}
