using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerContextDiscoveryStrategy
    {
        IEnumerable<(TriggerAdapterBase triggerAdapter, ITriggerContextDescriptor triggerContextDescriptor)> Discover(TriggerOptions options, ITriggerRegistryService triggerRegistryService, TriggerContextTracker tracker, ILogger logger);
    }
}
