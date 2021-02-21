using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveStartingTrigger
    {
        Task AfterSaveStarting(CancellationToken cancellationToken);
    }
}
