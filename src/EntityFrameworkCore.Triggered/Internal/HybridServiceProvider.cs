using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class HybridServiceProvider(IServiceProvider serviceProvider, DbContext dbContext) : IServiceProvider
    {
        readonly DbContext _dbContext = dbContext;
        readonly IServiceProvider _serviceProvider = serviceProvider;

        public object? GetService(Type serviceType)
        {
            // Starting from EF Core Triggered v3 we prefer the captured DbContext over our registry
            if (serviceType.IsAssignableFrom(_dbContext.GetType()))
            {
                return _dbContext;
            }

            var result = _serviceProvider.GetService(serviceType);
            if (result is not null)
            {
                return result;
            }

            return default;
        }
    }
}
