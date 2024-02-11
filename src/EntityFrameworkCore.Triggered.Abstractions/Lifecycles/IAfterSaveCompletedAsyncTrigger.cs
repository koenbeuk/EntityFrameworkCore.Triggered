namespace EntityFrameworkCore.Triggered.Lifecycles;

public interface IAfterSaveCompletedAsyncTrigger
{
    Task AfterSaveCompletedAsync(CancellationToken cancellationToken);
}
