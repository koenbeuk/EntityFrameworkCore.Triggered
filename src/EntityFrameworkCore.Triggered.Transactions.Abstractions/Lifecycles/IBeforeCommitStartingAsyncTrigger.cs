namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles
{
    public interface IBeforeCommitStartingAsyncTrigger
    {
        Task BeforeCommitStartingAsync(CancellationToken cancellationToken);
    }
}
