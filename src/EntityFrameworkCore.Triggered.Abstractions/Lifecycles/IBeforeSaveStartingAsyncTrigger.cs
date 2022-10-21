using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IBeforeSaveStartingAsyncTrigger
    {
        Task BeforeSaveStartingAsync(CancellationToken cancellationToken);
    }
}
