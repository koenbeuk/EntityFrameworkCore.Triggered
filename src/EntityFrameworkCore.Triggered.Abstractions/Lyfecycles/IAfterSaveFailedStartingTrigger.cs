using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lyfecycles
{
    public interface IAfterSaveFailedStartingTrigger
    {
        Task AfterSaveFailedStarting(Exception exception, CancellationToken cancellationToken);
    }
}
