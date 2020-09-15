using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
