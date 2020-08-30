using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerTypeDescriptor
    {
        Type TriggerType { get; }
        Task Invoke(object trigger, object triggerContext, Exception? exception, CancellationToken cancellationToken);
    }
}
