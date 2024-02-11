namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IAfterSaveFailedStartingAsyncTrigger
    {
        Task AfterSaveFailedStartingAsync(Exception exception, CancellationToken cancellationToken);
    }
}
