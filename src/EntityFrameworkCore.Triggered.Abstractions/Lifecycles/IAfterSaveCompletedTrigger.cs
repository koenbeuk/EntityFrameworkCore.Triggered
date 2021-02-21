using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveCompletedTrigger
    {
        Task AfterSaveCompleted(CancellationToken cancellationToken);
    }
}
