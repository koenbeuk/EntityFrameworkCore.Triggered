using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IAfterRollbackTrigger<in TEntity>
        where TEntity : class
    {
        void AfterRollback(ITriggerContext<TEntity> context);
    }
}
