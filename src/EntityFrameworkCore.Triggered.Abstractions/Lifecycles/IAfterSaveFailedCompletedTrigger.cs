namespace EntityFrameworkCore.Triggered.Lifecycles;

public interface IAfterSaveFailedCompletedTrigger
{
    void AfterSaveFailedCompleted(Exception exception);
}
