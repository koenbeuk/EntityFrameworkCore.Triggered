namespace EntityFrameworkCore.Triggered.Transactions.Lifecycles
{
    public interface IBeforeCommitCompletedTrigger
    {
        void BeforeCommitCompleted();
    }
}
