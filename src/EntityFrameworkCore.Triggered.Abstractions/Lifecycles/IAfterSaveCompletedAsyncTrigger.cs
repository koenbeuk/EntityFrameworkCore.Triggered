using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveCompletedAsyncTrigger
    {
        Task AfterSaveCompletedAsync(CancellationToken cancellationToken);
    }
}
