namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles;

public interface IBeforeCommitStartingTrigger
{
    void BeforeCommitStarting();
}
