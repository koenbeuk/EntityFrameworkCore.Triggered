using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveFailedCompletedTrigger
    {
        Task AfterSaveFailedCompleted(Exception exception, CancellationToken cancellationToken);
    }
}
