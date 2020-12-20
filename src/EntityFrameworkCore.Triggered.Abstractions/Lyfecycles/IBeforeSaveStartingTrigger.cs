using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lyfecycles
{
    public interface IBeforeSaveStartingTrigger
    {
        Task BeforeSaveStarting(CancellationToken cancellationToken);
    }
}
