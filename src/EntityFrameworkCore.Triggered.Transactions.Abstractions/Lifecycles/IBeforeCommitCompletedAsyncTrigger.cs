namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles
{
    public interface IBeforeCommitCompletedAsyncTrigger
    {
        Task BeforeCommitCompletedAsync(CancellationToken cancellationToken);
    }
}
