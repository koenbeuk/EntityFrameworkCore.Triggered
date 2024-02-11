namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveFailedCompletedAsyncTrigger
    {
        Task AfterSaveFailedCompletedAsync(Exception exception, CancellationToken cancellationToken);
    }
}
