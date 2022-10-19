using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveFailedCompletedAsyncTrigger
    {
        Task AfterSaveFailedCompletedAsync(Exception exception, CancellationToken cancellationToken);
    }
}
