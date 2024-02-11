namespace EntityFrameworkCore.Triggered.Internal.Descriptors;


public interface ITriggerTypeDescriptor
{
    Type TriggerType { get; }
    void Invoke(object trigger, object triggerContext, Exception? exception);
}
