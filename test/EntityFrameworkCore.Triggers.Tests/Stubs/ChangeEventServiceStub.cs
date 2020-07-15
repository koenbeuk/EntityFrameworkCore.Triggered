using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggers.Tests.Stubs
{
    [ExcludeFromCodeCoverage]
    public class ChangeEventServiceStub : IChangeEventService
    {
        public int CreateSessionCalls;

        public IChangeEventSession CreateSession(DbContext context)
        {
            CreateSessionCalls += 1;
            return new ChangeEventSessionStub();
        }
    }
}
