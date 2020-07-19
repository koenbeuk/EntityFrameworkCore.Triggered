using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerSessionStub : ITriggerSession
    {
        public int RaiseAfterSaveTriggersCalls;
        public int RaiseBeforeSaveTriggersCalls;

        public void DiscoverChanges()
        {

        }

        public Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveTriggersCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseBeforeSaveTriggers(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveTriggersCalls += 1;
            return Task.CompletedTask;
        }
    }
}
