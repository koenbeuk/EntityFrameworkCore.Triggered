using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions.Tests.Stubs
{
    public class TriggerStub<TEntity> : IBeforeSaveTrigger<TEntity>
        where TEntity : class
    {
        public Task BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
