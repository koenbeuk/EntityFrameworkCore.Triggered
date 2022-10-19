using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface IAfterSaveFailedAsyncTrigger<TEntity>
        where TEntity : class
    {
        Task AfterSaveFailedAsync(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken);
    }


}
