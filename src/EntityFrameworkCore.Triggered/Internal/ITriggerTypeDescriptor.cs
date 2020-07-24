using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerTypeDescriptor
    {
        Type TriggerType { get; }
        Task Invoke(object trigger, object triggerContext, CancellationToken cancellationToken);
    }
}
