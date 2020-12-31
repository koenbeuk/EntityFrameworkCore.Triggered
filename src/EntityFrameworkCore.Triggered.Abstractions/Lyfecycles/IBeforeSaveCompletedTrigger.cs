using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Lyfecycles
{
    public interface IBeforeSaveCompletedTrigger
    {
        Task BeforeSaveCompleted(CancellationToken cancellationToken);
    }
}
