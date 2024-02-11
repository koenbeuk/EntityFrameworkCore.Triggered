using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Tests.Stubs;

public class EventDefinitionBaseStub(ILoggingOptions loggingOptions, EventId eventId, LogLevel level, string eventIdCode) : EventDefinitionBase(loggingOptions, eventId, level, eventIdCode)
{
    public EventDefinitionBaseStub()
        : this(new LoggingOptions(), new EventId(), default, "test")
    {

    }
}
