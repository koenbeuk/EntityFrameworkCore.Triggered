using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerContextDiscoveryStrategy
    {
        IEnumerable<IEnumerable<TriggerContextDescriptor>> Discover(TriggerSessionConfiguration configuration, TriggerContextTracker tracker, ILogger logger);
    }
}
