using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggers.Tests.Stubs
{
    [ExcludeFromCodeCoverage]
    public class ChangeEventSessionStub : IChangeEventSession
    {
        public int RaiseAfterSaveChangeEventsCalls;
        public int RaiseBeforeSaveChangeEventsCalls;

        public Task RaiseAfterSaveChangeEvents(CancellationToken cancellationToken = default)
        {
            RaiseAfterSaveChangeEventsCalls += 1;
            return Task.CompletedTask;
        }

        public Task RaiseBeforeSaveChangeEvents(CancellationToken cancellationToken = default)
        {
            RaiseBeforeSaveChangeEventsCalls += 1;
            return Task.CompletedTask;
        }
    }
}
