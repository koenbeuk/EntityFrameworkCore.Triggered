namespace EntityFrameworkCore.Triggered;

public record TriggerSessionConfiguration
{
    public TriggerSessionConfiguration(bool disabled, int maxCascadeCycles)
    {
        Disabled = disabled;
        MaxCascadeCycles = maxCascadeCycles;
    }

    public bool Disabled { get; init; }

    public int MaxCascadeCycles { get; init; }
}
