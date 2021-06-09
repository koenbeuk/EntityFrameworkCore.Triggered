using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered.Extensions
{
    public static class DbContextExtensions
    {
        public static ITriggerSession CreateTriggerSession(this DbContext dbContext, IServiceProvider? serviceProvider = null)
        {
            var triggerService = dbContext.GetService<ITriggerService>() ?? throw new InvalidOperationException("Trigger service infrastructure is not configured");

            return triggerService.CreateSession(dbContext, serviceProvider);
        }
    }
}
