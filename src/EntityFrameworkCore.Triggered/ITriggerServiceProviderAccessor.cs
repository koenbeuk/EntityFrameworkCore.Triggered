namespace EntityFrameworkCore.Triggered;

public interface ITriggerServiceProviderAccessor
{
    IServiceProvider GetTriggerServiceProvider();
}
