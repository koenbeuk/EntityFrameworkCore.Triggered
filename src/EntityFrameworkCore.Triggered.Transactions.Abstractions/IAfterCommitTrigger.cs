using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IAfterCommitTrigger<in TEntity>
        where TEntity : class
    {
        Task AfterCommit(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
