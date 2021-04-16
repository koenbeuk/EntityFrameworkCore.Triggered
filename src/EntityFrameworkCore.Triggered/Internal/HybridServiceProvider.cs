using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var result = _serviceProvider.GetService(serviceType);
            if (result is not null)
            {
                return result;
            }

            if (typeof(DbContext).IsAssignableFrom(serviceType))
            {
                return _dbContext;
            }

            return default;
        }
    }
}
