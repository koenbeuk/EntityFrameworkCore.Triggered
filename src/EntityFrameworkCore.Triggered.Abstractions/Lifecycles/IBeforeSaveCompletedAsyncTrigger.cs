using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IBeforeSaveCompletedAsyncTrigger
    {
        Task BeforeSaveCompletedAsync(CancellationToken cancellationToken);
    }
}
