using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerSessionStub : ITriggerSession
    {
        public int RaiseBeforeSaveStartingTriggersCalls;
        public int RaiseBeforeSaveStartingAsyncTriggersCalls;
        public int RaiseBeforeSaveTriggersCalls;
        public int RaiseBeforeSaveAsyncTriggersCalls;
        public int RaiseBeforeSaveCompletingTriggersCalls;
        public int RaiseBeforeSaveCompletingAsyncTriggersCalls;
        public int RaiseAfterSaveStartingTriggersCalls;
        public int RaiseAfterSaveStartingAsyncTriggersCalls;
        public int RaiseAfterSaveTriggersCalls;
        public int RaiseAfterSaveAsyncTriggersCalls;
        public int RaiseAfterSaveCompletedTriggersCalls;
        public int RaiseAfterSaveCompletedAsyncTriggersCalls;
        public int RaiseAfterSaveFailedStartingTriggersCalls;
        public int RaiseAfterSaveFailedStartingAsyncTriggersCalls;
        public int RaiseAfterSaveFailedTriggersCalls;
        public int RaiseAfterSaveFailedAsyncTriggersCalls;
        public int RaiseAfterSaveFailedCompletedTriggersCalls;
        public int RaiseAfterSaveFailedCompletedAsyncTriggersCalls;
        public int CaptureDiscoveredChangesCalls;
        public int DiscoverChangesCalls;
        public int DisposeCalls;

        public void CaptureDiscoveredChanges() => CaptureDiscoveredChangesCalls += 1;

        public void DiscoverChanges() => DiscoverChangesCalls += 1;

        public void Dispose()
        {
            DisposeCalls += 1;
        }

        public void RaiseAfterSaveFailedStartingTriggers(Exception exception)
        {
            RaiseAfterSaveFailedStartingTriggersCalls += 1;
        }

        public Task RaiseAfterSaveFailedStartingAsyncTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveFailedStartingAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveFailedTriggers(Exception exception)
        {
            RaiseAfterSaveFailedTriggersCalls += 1;
        }

        public Task RaiseAfterSaveFailedAsyncTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveFailedAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveFailedCompletedTriggers(Exception exception)
        {
            RaiseAfterSaveFailedCompletedTriggersCalls += 1;
        }

        public Task RaiseAfterSaveFailedCompletedAsyncTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveFailedCompletedAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseBeforeSaveStartingTriggers()
        {
            RaiseBeforeSaveStartingTriggersCalls += 1;
        }

        public Task RaiseBeforeSaveStartingAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveStartingAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseBeforeSaveTriggers()
        {
            RaiseBeforeSaveTriggersCalls += 1;
        }

        public Task RaiseBeforeSaveAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseBeforeSaveCompletedTriggers()
        {
            RaiseBeforeSaveCompletingTriggersCalls += 1;
        }

        public Task RaiseBeforeSaveCompletedAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveCompletingAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveStartingTriggers()
        {
            RaiseAfterSaveStartingTriggersCalls += 1;
        }

        public Task RaiseAfterSaveStartingAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveStartingAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveTriggers()
        {
            RaiseAfterSaveTriggersCalls += 1;
        }

        public Task RaiseAfterSaveAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveCompletedTriggers()
        {
            RaiseAfterSaveCompletedTriggersCalls += 1;
        }

        public Task RaiseAfterSaveCompletedAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveCompletedAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseBeforeSaveTriggers(bool skipDetectedChanges = false)
        {
            RaiseBeforeSaveTriggersCalls += 1;
        }

        public Task RaiseBeforeSaveAsyncTriggers(bool skipDetectedChanges = false, CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }
    }
}
