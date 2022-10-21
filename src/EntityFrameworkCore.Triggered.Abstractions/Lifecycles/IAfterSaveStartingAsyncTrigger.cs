using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveStartingAsyncTrigger
    {
        Task AfterSaveStartingAsync(CancellationToken cancellationToken);
    }
}
