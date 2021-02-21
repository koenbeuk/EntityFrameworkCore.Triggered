using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IBeforeSaveCompletedTrigger
    {
        Task BeforeSaveCompleted(CancellationToken cancellationToken);
    }
}
