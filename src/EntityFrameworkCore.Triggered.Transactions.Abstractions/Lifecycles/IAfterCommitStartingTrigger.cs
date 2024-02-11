namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles
{
    public interface IAfterCommitStartingTrigger
    {
        void AfterCommitStarting();
    }
}
