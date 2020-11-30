using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lyfecycles
{
    public interface IAfterSaveStartedTrigger
    {
        Task AfterSaveStarted(CancellationToken cancellationToken);
    }
}
