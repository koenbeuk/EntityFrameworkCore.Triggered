namespace EntityFrameworkCore.Triggered.Lifecycles
{
    public interface IBeforeSaveStartingAsyncTrigger
    {
        Task BeforeSaveStartingAsync(CancellationToken cancellationToken);
    }
}
