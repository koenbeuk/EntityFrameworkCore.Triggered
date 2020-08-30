using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface IBeforeSaveTrigger<in TEntity>
        where TEntity : class
    {
        Task BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
