using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IAfterRollbackTrigger<in TEntity>
        where TEntity : class
    {
        Task AfterRollback(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
