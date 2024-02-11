namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveFailedStartingTrigger
    {
        void AfterSaveFailedStarting(Exception exception);
    }
}
