namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles;

public interface IAfterCommitCompletedTrigger
{
    void AfterCommitCompleted();
}
