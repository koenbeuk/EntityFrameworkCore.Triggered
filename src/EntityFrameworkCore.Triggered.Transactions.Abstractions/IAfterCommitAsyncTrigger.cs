using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IAfterCommitAsyncTrigger<in TEntity>
        where TEntity : class
    {
        Task AfterCommitAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
