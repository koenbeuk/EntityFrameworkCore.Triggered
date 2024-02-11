namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles;

public interface IAfterCommitCompletedAsyncTrigger
{
    Task AfterCommitCompletedAsync(CancellationToken cancellationToken);
}
