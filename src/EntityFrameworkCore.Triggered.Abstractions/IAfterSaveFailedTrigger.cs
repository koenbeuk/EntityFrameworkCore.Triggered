using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface IAfterSaveFailedTrigger<TEntity>
        where TEntity : class
    {
        Task AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken);
    }
}
