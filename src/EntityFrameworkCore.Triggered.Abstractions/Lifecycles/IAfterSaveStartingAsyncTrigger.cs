namespace EntityFrameworkCore.Triggered.Lifecycles;

public interface IAfterSaveStartingAsyncTrigger
{
    Task AfterSaveStartingAsync(CancellationToken cancellationToken);
}
