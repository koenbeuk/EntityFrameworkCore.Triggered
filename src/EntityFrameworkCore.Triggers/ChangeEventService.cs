using System;
using EntityFrameworkCore.Triggers.Internal;
using EntityFrameworkCore.Triggers.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggers
{
    public class ChangeEventService : IChangeEventService
    {
        readonly IChangeEventHandlerRegistryService _changeEventHandlerRegistryService;
        readonly IRecursionStrategy _recursionStrategy;
        readonly ILoggerFactory _loggerFactory;

        public ChangeEventService(IChangeEventHandlerRegistryService changeEventHandlerRegistryService, IRecursionStrategy recursionStrategy, ILoggerFactory loggerFactory)
        {
            _changeEventHandlerRegistryService = changeEventHandlerRegistryService ?? throw new ArgumentNullException(nameof(changeEventHandlerRegistryService));
            _recursionStrategy = recursionStrategy ?? throw new ArgumentNullException(nameof(recursionStrategy));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public IChangeEventSession CreateSession(DbContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var changeEventTracker = new ChangeEventTracker(context.ChangeTracker, _recursionStrategy);

            return new ChangeEventSession(_changeEventHandlerRegistryService, changeEventTracker, _loggerFactory);
        }
    }
}
