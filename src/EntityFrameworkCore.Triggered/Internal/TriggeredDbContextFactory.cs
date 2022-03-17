using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggeredDbContextFactory<TContext, TFactory> : IDbContextFactory<TContext>
        where TContext : DbContext
        where TFactory : IDbContextFactory<TContext>
    {
        private readonly TFactory _contextFactory;
        private readonly IServiceProvider _serviceProvider;

        public TriggeredDbContextFactory(TFactory contextFactory, IServiceProvider serviceProvider)
        {
            _contextFactory = contextFactory;
            _serviceProvider = serviceProvider;
        }

        public TContext CreateDbContext()
        {
            var context = _contextFactory.CreateDbContext();
            Debug.Assert(context != null);

            var applicationTriggerServiceProviderAccessor = context.GetService<ApplicationTriggerServiceProviderAccessor>();
            if (applicationTriggerServiceProviderAccessor != null)
            {
                applicationTriggerServiceProviderAccessor.SetTriggerServiceProvider(new HybridServiceProvider(_serviceProvider, context));
            }

            return context;
        }
    }
    
    public sealed class TriggeredDbContextFactory<TContext> : IDbContextFactory<TContext>
        where TContext : DbContext
    {
        readonly Func<IServiceProvider, IDbContextFactory<TContext>> _contextFactoryFactory;
        readonly IServiceProvider _serviceProvider;

        public TriggeredDbContextFactory(Func<IServiceProvider, IDbContextFactory<TContext>> contextFactoryFactory, IServiceProvider serviceProvider)
        {
            _contextFactoryFactory = contextFactoryFactory;
            _serviceProvider = serviceProvider;
        }

        public TContext CreateDbContext()
        {
            var contextFactory = _contextFactoryFactory(_serviceProvider);
            var context = contextFactory.CreateDbContext();
            Debug.Assert(context is not null);

            var applicationTriggerServiceProviderAccessor = context.GetService<ApplicationTriggerServiceProviderAccessor>();
            if (applicationTriggerServiceProviderAccessor != null)
            {
                applicationTriggerServiceProviderAccessor.SetTriggerServiceProvider(new HybridServiceProvider(_serviceProvider, context));
            }

            return context;
        }
    }
}
