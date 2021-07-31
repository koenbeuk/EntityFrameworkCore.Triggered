using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Gets the <c>ITriggerService</c> that is used to create trigger sessions.Allows for just in time configuration
        /// </summary>
        public static ITriggerService GetTriggerService(this DbContext dbContext)
        {
            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            return dbContext.GetService<ITriggerService>() ?? throw new InvalidOperationException("Triggers are not configured for this DbContext");
        }

        /// <summary>
        /// Gets or creates a <c>ITriggerSession</c> that can be used to manually invoke triggers
        /// </summary>
        public static ITriggerSession CreateTriggerSession(this DbContext dbContext, IServiceProvider? serviceProvider = null)
        {
            var triggerService = GetTriggerService(dbContext);

            return triggerService.CreateSession(dbContext, serviceProvider);
        }

        /// <summary>
        /// Calls dbContext.SaveChanges without invoking triggers
        /// </summary>
        public static int SaveChangesWithoutTriggers(this DbContext dbContext, bool acceptAllChangesOnSuccess = true)
        {
            var triggerService = GetTriggerService(dbContext);
            var initialConfiguration = triggerService.Configuration;

            try
            {
                triggerService.Configuration = initialConfiguration with { Disabled = true };

                return dbContext.SaveChanges(acceptAllChangesOnSuccess);
            }
            finally
            {
                triggerService.Configuration = initialConfiguration;
            }
        }

        /// <summary>
        /// Calls dbContext.SaveChanges without invoking triggers
        /// </summary>
        public static Task<int> SaveChangesWithoutTriggersAsync(this DbContext dbContext, CancellationToken cancellationToken = default)
            => SaveChangesWithoutTriggersAsync(dbContext, true, cancellationToken);

        /// <summary>
        /// Calls dbContext.SaveChanges without invoking triggers
        /// </summary>
        public static Task<int> SaveChangesWithoutTriggersAsync(this DbContext dbContext, bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var triggerService = GetTriggerService(dbContext);
            var initialConfiguration = triggerService.Configuration;

            try
            {
                triggerService.Configuration = initialConfiguration with { Disabled = true };

                return dbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            finally
            {
                triggerService.Configuration = initialConfiguration;
            }
        }
    }
}
