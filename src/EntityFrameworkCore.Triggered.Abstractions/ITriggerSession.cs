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
        /// Discoveres any new pending changes in the DbContext
        /// </summary>
        void DiscoverChanges();
        /// <summary>
        /// Makes a snapshot of all changes in the DbContext and invokes BeforeSaveTriggers recursively based on the recursive settings until all changes have been processed
        /// </summary>
        Task RaiseBeforeSaveTriggers(CancellationToken cancellationToken = default);
        /// <summary>
        /// Makes a snapshot of all changes in the DbContext and invokes BeforeSaveTriggers recursively based on the recursive settings until all changes have been processed
        /// </summary>
        /// <param name="skipDetectedChanges">Allows BeforeSaveTriggers not to include previously detected changes. Only new changes will be detected and fired upon. This is useful in case of multiple calls to RaiseBeforeSaveTriggers</param>
        Task RaiseBeforeSaveTriggers(bool skipDetectedChanges, CancellationToken cancellationToken = default);
        /// <summary>
        /// Captures and locks all discovered changes
        /// </summary>
        void CaptureDiscoveredChanges();
        /// <summary>
        /// Invokes AfterSaveTriggers non-recursively. Calling this method expects that either RaiseBeforeSaveTriggers() or DiscoverChanges() has been called
        /// </summary>
        Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default);
        /// <summary>
        /// Invokes AfterSaveFailedTriggers non-recursively. Calling this method expects that either RaiseBeforeSaveTriggers() or DiscoverChanges() has been called
        /// </summary>
        Task RaiseAfterSaveFailedTriggers(Exception exception, CancellationToken cancellationToken = default);
    }
}
