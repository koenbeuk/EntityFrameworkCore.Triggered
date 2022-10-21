using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveFailedStartingAsyncTrigger
    {
        Task AfterSaveFailedStartingAsync(Exception exception, CancellationToken cancellationToken);
    }
}
