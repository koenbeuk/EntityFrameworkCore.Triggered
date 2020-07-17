using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    [ExcludeFromCodeCoverage]
    public class TriggerServiceStub : ITriggerService
    {
        public int CreateSessionCalls;

        public ITriggerSession CreateSession(DbContext context)
        {
            CreateSessionCalls += 1;
            return new TriggerSessionStub();
        }
    }
}
