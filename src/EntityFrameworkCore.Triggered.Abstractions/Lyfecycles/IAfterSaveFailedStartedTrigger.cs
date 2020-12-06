using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lyfecycles
{
    public interface IAfterSaveFailedStartedTrigger
    {
        Task AfterSaveFailedStarted(Exception exception, CancellationToken cancellationToken);
    }
}
