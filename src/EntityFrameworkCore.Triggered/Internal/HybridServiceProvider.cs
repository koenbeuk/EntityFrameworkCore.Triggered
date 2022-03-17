using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class HybridServiceProvider : IServiceProvider
    {
        readonly DbContext _dbContext;
        readonly IServiceProvider _serviceProvider;

        public HybridServiceProvider(IServiceProvider serviceProvider, DbContext dbContext)
        {
            _serviceProvider = serviceProvider;
            _dbContext = dbContext;
        }

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
