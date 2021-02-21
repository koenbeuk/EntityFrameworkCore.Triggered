using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IBeforeSaveStartingTrigger
    {
        Task BeforeSaveStarting(CancellationToken cancellationToken);
    }
}
