using System;
using EntityFrameworkCore.Triggers.Internal;
using EntityFrameworkCore.Triggers.Internal.RecursionStrategy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Triggers
{
    public class TriggerService : ITriggerService
    {
        readonly ITriggerRegistryService _triggerRegistryService;
        readonly IRecursionStrategy _recursionStrategy;
        readonly ILoggerFactory _loggerFactory;
        readonly TriggerOptions _options;

        public TriggerService(ITriggerRegistryService triggerRegistryService, IRecursionStrategy recursionStrategy, ILoggerFactory loggerFactory, IOptionsSnapshot<TriggerOptions> triggerOptionsSnapshot)
        {
            _triggerRegistryService = triggerRegistryService ?? throw new ArgumentNullException(nameof(triggerRegistryService));
            _recursionStrategy = recursionStrategy ?? throw new ArgumentNullException(nameof(recursionStrategy));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _options = triggerOptionsSnapshot.Value;
        }

        public ITriggerSession CreateSession(DbContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var triggerContextTracker = new TriggerContextTracker(context.ChangeTracker, _recursionStrategy);

            return new TriggerSession(_options, _triggerRegistryService, triggerContextTracker, _loggerFactory);
        }
    }
}
