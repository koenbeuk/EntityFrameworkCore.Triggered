using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerSessionStub : ITriggerSession
    {
        public int RaiseBeforeSaveStartingTriggersCalls;
        public int RaiseBeforeSaveTriggersCalls;
        public int RaiseBeforeSaveStartedTriggersCalls;
        public int RaiseAfterSaveStartingTriggersCalls;
        public int RaiseAfterSaveTriggersCalls;
        public int RaiseAfterSaveStartedTriggersCalls;
        public int RaiseAfterSaveFailedStartingTriggersCalls;
        public int RaiseAfterSaveFailedTriggersCalls;
        public int RaiseAfterSaveFailedStartedTriggersCalls;
        public int CaptureDiscoveredChangesCalls;
        public int DiscoverChangesCalls;
        public int DisposeCalls;

        public void CaptureDiscoveredChanges() => CaptureDiscoveredChangesCalls += 1;

        public void DiscoverChanges() => DiscoverChangesCalls += 1;

        public void Dispose()
        {
            DisposeCalls += 1;
        }

        public Task RaiseAfterSaveFailedStartingTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveFailedStartingTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseAfterSaveFailedTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveFailedTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseAfterSaveFailedStartedTriggers(Exception exception, CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveFailedStartedTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseBeforeSaveStartingTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveStartingTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseBeforeSaveTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseBeforeSaveStartedTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveStartedTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseAfterSaveStartingTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveStartingTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseAfterSaveStartedTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveStartedTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseBeforeSaveTriggers(bool skipDetectedChanges = false, CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveTriggersCalls += 1;
            return Task.CompletedTask;
        }
    }
}
