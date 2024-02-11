namespace EntityFrameworkCore.Triggered.Lifecycles;

public interface IBeforeSaveCompletedTrigger
{
    void BeforeSaveCompleted();
}
