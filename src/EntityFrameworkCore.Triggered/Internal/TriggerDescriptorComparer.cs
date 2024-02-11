namespace EntityFrameworkCore.Triggered.Internal;

public sealed class TriggerDescriptorComparer : IComparer<TriggerDescriptor>, IComparer<AsyncTriggerDescriptor>
{
    public int Compare(TriggerDescriptor? x, TriggerDescriptor? y)
    {
        ArgumentNullException.ThrowIfNull(x);

        ArgumentNullException.ThrowIfNull(y);

        return x.Priority - y.Priority;
    }


    public int Compare(AsyncTriggerDescriptor? x, AsyncTriggerDescriptor? y)
    {
        ArgumentNullException.ThrowIfNull(x);

        ArgumentNullException.ThrowIfNull(y);

        return x.Priority - y.Priority;
    }
}
