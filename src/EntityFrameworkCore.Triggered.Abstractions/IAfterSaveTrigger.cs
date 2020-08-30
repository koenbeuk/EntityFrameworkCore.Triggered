using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface IAfterSaveTrigger<TEntity>
        where TEntity : class
    {
        Task AfterSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
