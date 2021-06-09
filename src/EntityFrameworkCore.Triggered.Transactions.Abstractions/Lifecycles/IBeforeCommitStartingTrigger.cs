using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles
{
    public interface IBeforeCommitStartingTrigger
    {
        Task BeforeCommitStarting(CancellationToken cancellationToken);
    }
}
