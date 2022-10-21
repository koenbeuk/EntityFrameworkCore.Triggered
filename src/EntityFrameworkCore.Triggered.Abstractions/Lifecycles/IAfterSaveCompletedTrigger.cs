namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveCompletedTrigger
    {
        void AfterSaveCompleted();
    }
}
