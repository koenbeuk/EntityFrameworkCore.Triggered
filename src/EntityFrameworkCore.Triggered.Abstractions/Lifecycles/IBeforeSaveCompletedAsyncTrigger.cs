namespace EntityFrameworkCore.Triggered.Lifecycles;

public interface IBeforeSaveCompletedAsyncTrigger
{
    Task BeforeSaveCompletedAsync(CancellationToken cancellationToken);
}
