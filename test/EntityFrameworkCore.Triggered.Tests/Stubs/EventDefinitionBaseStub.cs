using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class EventDefinitionBaseStub : EventDefinitionBase
    {
        public EventDefinitionBaseStub()
            : this(new LoggingOptions(), new EventId(), default, "test")
        {

        }

        public EventDefinitionBaseStub(ILoggingOptions loggingOptions, EventId eventId, LogLevel level, string eventIdCode) : base(loggingOptions, eventId, level, eventIdCode)
        {
        }
    }
}
