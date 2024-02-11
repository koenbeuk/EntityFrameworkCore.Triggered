using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerSessionStub : ITriggerSession
    {
        public int RaiseBeforeSaveStartingTriggersCalls { get; set; }
        public int RaiseBeforeSaveStartingAsyncTriggersCalls { get; set; }
        public int RaiseBeforeSaveTriggersCalls { get; set; }
        public int RaiseBeforeSaveAsyncTriggersCalls { get; set; }
        public int RaiseBeforeSaveCompletingTriggersCalls { get; set; }
        public int RaiseBeforeSaveCompletingAsyncTriggersCalls { get; set; }
        public int RaiseAfterSaveStartingTriggersCalls { get; set; }
        public int RaiseAfterSaveStartingAsyncTriggersCalls { get; set; }
        public int RaiseAfterSaveTriggersCalls { get; set; }
        public int RaiseAfterSaveAsyncTriggersCalls { get; set; }
        public int RaiseAfterSaveCompletedTriggersCalls { get; set; }
        public int RaiseAfterSaveCompletedAsyncTriggersCalls { get; set; }
        public int RaiseAfterSaveFailedStartingTriggersCalls { get; set; }
        public int RaiseAfterSaveFailedStartingAsyncTriggersCalls { get; set; }
        public int RaiseAfterSaveFailedTriggersCalls { get; set; }
        public int RaiseAfterSaveFailedAsyncTriggersCalls { get; set; }
        public int RaiseAfterSaveFailedCompletedTriggersCalls { get; set; }
        public int RaiseAfterSaveFailedCompletedAsyncTriggersCalls { get; set; }
        public int CaptureDiscoveredChangesCalls { get; set; }
        public int DiscoverChangesCalls { get; set; }
        public int DisposeCalls { get; set; }

        public void CaptureDiscoveredChanges() => CaptureDiscoveredChangesCalls += 1;

        public void DiscoverChanges() => DiscoverChangesCalls += 1;

        public void Dispose() => DisposeCalls += 1;

        public void RaiseAfterSaveFailedStartingTriggers(Exception exception) => RaiseAfterSaveFailedStartingTriggersCalls += 1;

        public Task RaiseAfterSaveFailedStartingAsyncTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveFailedStartingAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveFailedTriggers(Exception exception) => RaiseAfterSaveFailedTriggersCalls += 1;

        public Task RaiseAfterSaveFailedAsyncTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveFailedAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveFailedCompletedTriggers(Exception exception) => RaiseAfterSaveFailedCompletedTriggersCalls += 1;

        public Task RaiseAfterSaveFailedCompletedAsyncTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveFailedCompletedAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseBeforeSaveStartingTriggers() => RaiseBeforeSaveStartingTriggersCalls += 1;

        public Task RaiseBeforeSaveStartingAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveStartingAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseBeforeSaveTriggers() => RaiseBeforeSaveTriggersCalls += 1;

        public Task RaiseBeforeSaveAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseBeforeSaveCompletedTriggers() => RaiseBeforeSaveCompletingTriggersCalls += 1;

        public Task RaiseBeforeSaveCompletedAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveCompletingAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveStartingTriggers() => RaiseAfterSaveStartingTriggersCalls += 1;

        public Task RaiseAfterSaveStartingAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveStartingAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveTriggers() => RaiseAfterSaveTriggersCalls += 1;

        public Task RaiseAfterSaveAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseAfterSaveCompletedTriggers() => RaiseAfterSaveCompletedTriggersCalls += 1;

        public Task RaiseAfterSaveCompletedAsyncTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveCompletedAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public void RaiseBeforeSaveTriggers(bool skipDetectedChanges = false) => RaiseBeforeSaveTriggersCalls += 1;

        public Task RaiseBeforeSaveAsyncTriggers(bool skipDetectedChanges = false, CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveAsyncTriggersCalls += 1;
            return Task.CompletedTask;
        }
    }
}
