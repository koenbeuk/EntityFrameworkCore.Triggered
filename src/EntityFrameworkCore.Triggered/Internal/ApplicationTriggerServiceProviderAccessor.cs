using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class ApplicationTriggerServiceProviderAccessor : ITriggerServiceProviderAccessor, IDisposable
    {
        readonly IServiceProvider _serviceProvider;
        readonly IServiceScope? _serviceScope;


        public ApplicationTriggerServiceProviderAccessor(IServiceProvider internalServiceProvider, Func<IServiceProvider, IServiceProvider>? scopedServiceProviderTransform)
        {
            var dbContextOptions = internalServiceProvider.GetRequiredService<IDbContextOptions>();
            var coreOptionsExtension = dbContextOptions.FindExtension<CoreOptionsExtension>();

            var applicationServiceProvider = coreOptionsExtension.ApplicationServiceProvider;
            if (applicationServiceProvider != null)
            {
                if (scopedServiceProviderTransform != null)
                {
                    _serviceProvider = scopedServiceProviderTransform(applicationServiceProvider);
                }
                else
                {
                    _serviceScope = applicationServiceProvider.CreateScope();
                    _serviceProvider = _serviceScope.ServiceProvider;
                }
            }
            else
            {
                _serviceProvider = internalServiceProvider;
            }
        }

        public IServiceProvider GetTriggerServiceProvider()
            => _serviceProvider;

        public void Dispose()
        {
            _serviceScope?.Dispose();
        }
    }
}
