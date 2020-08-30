using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerServiceStub : ITriggerService
    {
        public int CreateSessionCalls;
        public IServiceProvider ServiceProvider;
        public TriggerSessionStub LastSession;

        public ITriggerSession CreateSession(DbContext context, IServiceProvider serviceProvider)
        {
            CreateSessionCalls += 1;
            ServiceProvider = serviceProvider;
            LastSession = new TriggerSessionStub();
            return LastSession;
        }
    }
}
