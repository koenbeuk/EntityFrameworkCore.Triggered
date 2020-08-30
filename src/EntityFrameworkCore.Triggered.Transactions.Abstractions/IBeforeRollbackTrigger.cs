using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IBeforeRollbackTrigger<in TEntity>
        where TEntity : class
    {
        Task BeforeRollback(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
