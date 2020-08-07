using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerSession
    {
        /// <summary>
        /// Captures any pending changes in the DbContext
        /// </summary>
        /// <param name="skipDetectedChanges">Allows BeforeSaveTriggers not to include previously detected changes. Only new changes will be detected and fired upon. This is useful in case of multiple calls to RaiseBeforeSaveTriggers</param>
        /// <remarks>
        /// Some triggers, like AfterSaveTriggers need help in knowing what has changed since by the time they are raised, the DbContext should have already committed all pending changes
        /// Normally this is done by calling RaiseBeforeSaveTriggers prior to the DbContext committing on its changes however in case that is not possible, DiscoverChanges can be used to make a snapshot
        /// </remarks>
        void DiscoverChanges();
        /// <summary>
        /// Makes a snapshot of all changes in the DbContext and invokes BeforeSaveTriggers recursively based on the recursive settings until all changes have been processed
        /// </summary>
        Task RaiseBeforeSaveTriggers(CancellationToken cancellationToken = default, bool skipDetectedChanges = false);
        /// <summary>
        /// Invokes AfterSaveTriggers non-recursively. Calling this method expects that either RaiseBeforeSaveTriggers() or DiscoverChanges() has called
        /// </summary>
        /// <returns></returns>
        Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default);
    }
}
