namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerSession : IDisposable
    {
        /// <summary>
        /// Discoveres any new pending changes in the DbContext
        /// </summary>
        void DiscoverChanges();
        void RaiseBeforeSaveStartingTriggers();
        void RaiseBeforeSaveCompletedTriggers();
        void RaiseAfterSaveFailedStartingTriggers(Exception exception);
        void RaiseAfterSaveFailedCompletedTriggers(Exception exception);
        void RaiseAfterSaveStartingTriggers();
        void RaiseAfterSaveCompletedTriggers();
        Task RaiseBeforeSaveStartingAsyncTriggers(CancellationToken cancellationToken = default);
        Task RaiseBeforeSaveCompletedAsyncTriggers(CancellationToken cancellationToken = default);
        Task RaiseAfterSaveFailedStartingAsyncTriggers(Exception exception, CancellationToken cancellationToken = default);
        Task RaiseAfterSaveFailedCompletedAsyncTriggers(Exception exception, CancellationToken cancellationToken = default);
        Task RaiseAfterSaveStartingAsyncTriggers(CancellationToken cancellationToken = default);
        Task RaiseAfterSaveCompletedAsyncTriggers(CancellationToken cancellationToken = default);
        /// <summary>
        /// Makes a snapshot of all changes in the DbContext and invokes BeforeSaveTriggers while detecting and cascading based on the cascade settings until all changes have been processed
        /// </summary>
        void RaiseBeforeSaveTriggers();
        /// <summary>
        /// Makes a snapshot of all changes in the DbContext and invokes BeforeSaveTriggers while detecting and cascading based on the cascade settings until all changes have been processed
        /// </summary>
        Task RaiseBeforeSaveAsyncTriggers(CancellationToken cancellationToken = default);
        /// <summary>
        /// Makes a snapshot of all changes in the DbContext and invokes BeforeSaveTriggers while detecting and cascading based on the cascade settings until all changes have been processed
        /// </summary>
        /// <param name="skipDetectedChanges">Allows BeforeSaveTriggers not to include previously detected changes. Only new changes will be detected and fired upon. This is useful in case of multiple calls to RaiseBeforeSaveTriggers</param>
        void RaiseBeforeSaveTriggers(bool skipDetectedChanges);
        /// <summary>
        /// Makes a snapshot of all changes in the DbContext and invokes BeforeSaveTriggers while detecting and cascading based on the cascade settings until all changes have been processed
        /// </summary>
        /// <param name="skipDetectedChanges">Allows BeforeSaveTriggers not to include previously detected changes. Only new changes will be detected and fired upon. This is useful in case of multiple calls to RaiseBeforeSaveTriggers</param>
        Task RaiseBeforeSaveAsyncTriggers(bool skipDetectedChanges, CancellationToken cancellationToken = default);
        /// <summary>
        /// Captures and locks all discovered changes
        /// </summary>
        void CaptureDiscoveredChanges();
        /// <summary>
        /// Invokes AfterSaveTriggers. Calling this method expects that either RaiseBeforeSaveTriggers() or DiscoverChanges() has been called
        /// </summary>
        void RaiseAfterSaveTriggers();
        /// <summary>
        /// Invokes AfterSaveTriggers. Calling this method expects that either RaiseBeforeSaveTriggers() or DiscoverChanges() has been called
        /// </summary>
        Task RaiseAfterSaveAsyncTriggers(CancellationToken cancellationToken = default);
        /// <summary>
        /// Invokes AfterSaveFailedTriggers. Calling this method expects that either RaiseBeforeSaveTriggers() or DiscoverChanges() has been called
        /// </summary>
        void RaiseAfterSaveFailedTriggers(Exception exception);
        /// <summary>
        /// Invokes AfterSaveFailedTriggers. Calling this method expects that either RaiseBeforeSaveTriggers() or DiscoverChanges() has been called
        /// </summary>
        Task RaiseAfterSaveFailedAsyncTriggers(Exception exception, CancellationToken cancellationToken = default);
    }
}
