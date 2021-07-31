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
        /// Creates a <c>ITriggerSession</c> that can be used to manually invoke triggers
        /// </summary>
        public static ITriggerSession CreateTriggerSession(this DbContext dbContext, Func<TriggerSessionConfiguration, TriggerSessionConfiguration> configurator, IServiceProvider? serviceProvider = null)
        {
            var triggerService = GetTriggerService(dbContext);
            var configuration = configurator(triggerService.Configuration);

            return triggerService.CreateSession(dbContext, configuration, serviceProvider);
        }


        /// <summary>
        /// Creates a new <c>ITriggerSession</c> that can be used to manually invoke triggers. Throws if a TriggerSession is already active
        /// </summary>
        public static ITriggerSession CreateNewTriggerSession(this DbContext dbContext, Func<TriggerSessionConfiguration, TriggerSessionConfiguration>? configurator = null, IServiceProvider? serviceProvider = null)
        {
            var triggerService = GetTriggerService(dbContext);
            if (triggerService.Current is not null)
            {
                throw new InvalidOperationException("A triggerSession has already been created");
            }

            var configuration = configurator?.Invoke(triggerService.Configuration) ?? triggerService.Configuration;

            return triggerService.CreateSession(dbContext, configuration, serviceProvider);
        }

        /// <summary>
        /// Calls dbContext.SaveChanges without invoking triggers
        /// </summary>
        public static int SaveChangesWithoutTriggers(this DbContext dbContext, bool acceptAllChangesOnSuccess = true)
        {
            CreateNewTriggerSession(dbContext, configuration => configuration with {
                Disabled = true
            });

            return dbContext.SaveChanges(acceptAllChangesOnSuccess);
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
            CreateNewTriggerSession(dbContext, configuration => configuration with {
                Disabled = true
            });

            return dbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
