using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lyfecycles
{
    public interface IAfterSaveFailedCompletedTrigger
    {
        Task AfterSaveFailedCompleted(Exception exception, CancellationToken cancellationToken);
    }
}
