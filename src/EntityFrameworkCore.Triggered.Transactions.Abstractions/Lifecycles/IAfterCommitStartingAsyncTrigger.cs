namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles;

public interface IAfterCommitStartingAsyncTrigger
{
    Task AfterCommitStartingAsync(CancellationToken cancellationToken);
}
