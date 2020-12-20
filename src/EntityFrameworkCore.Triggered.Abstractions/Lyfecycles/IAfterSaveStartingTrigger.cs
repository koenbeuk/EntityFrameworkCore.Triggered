using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lyfecycles
{
    public interface IAfterSaveStartingTrigger
    {
        Task AfterSaveStarting(CancellationToken cancellationToken);
    }
}
