using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class ApplicationTriggerServiceProviderAccessor : ITriggerServiceProviderAccessor, IDisposable, IResettableService
    {
        readonly IServiceProvider _internalServiceProvider;
        readonly IServiceProvider? _fallbackApplicationServiceProvider;
        readonly Func<IServiceProvider, IServiceProvider>? _scopedServiceProviderTransform;

        IServiceScope? _serviceScope;
        IServiceProvider? _applicationScopedServiceProvider;

        public ApplicationTriggerServiceProviderAccessor(IServiceProvider internalServiceProvider, Func<IServiceProvider, IServiceProvider>? scopedServiceProviderTransform)
        {
            if (internalServiceProvider is null)
            {
                throw new ArgumentNullException(nameof(internalServiceProvider));
            }

            var dbContextOptions = internalServiceProvider.GetRequiredService<IDbContextOptions>();
            var coreOptionsExtension = dbContextOptions.FindExtension<CoreOptionsExtension>();

            _internalServiceProvider = internalServiceProvider;
            if (scopedServiceProviderTransform == null)
            {
                _fallbackApplicationServiceProvider = coreOptionsExtension.ApplicationServiceProvider;
            }
            
            _scopedServiceProviderTransform = scopedServiceProviderTransform;
        }

        public void SetTriggerServiceProvider(IServiceProvider serviceProvider)
        {
            if (_applicationScopedServiceProvider != null)
            {
                throw new InvalidOperationException("Can only set applicationScopedServiceProvider once");
            }

            _applicationScopedServiceProvider = serviceProvider;
        }

        public IServiceProvider GetTriggerServiceProvider()
        {
            if (_applicationScopedServiceProvider == null)
            {
                if (_fallbackApplicationServiceProvider != null)
                {
                    _applicationScopedServiceProvider = _fallbackApplicationServiceProvider;
                }
                else if (_scopedServiceProviderTransform != null)
                {

                    var dbContextOptions = _internalServiceProvider.GetRequiredService<IDbContextOptions>();
                    var coreOptionsExtension = dbContextOptions.FindExtension<CoreOptionsExtension>();
                    var serviceProvider = coreOptionsExtension.ApplicationServiceProvider ?? _internalServiceProvider;

                    _applicationScopedServiceProvider = _scopedServiceProviderTransform(serviceProvider);
                }
                else
                {
                    _serviceScope = _internalServiceProvider.CreateScope();
                    _applicationScopedServiceProvider = _serviceScope.ServiceProvider;
                }

            }

            return _applicationScopedServiceProvider;
        }

        public void Dispose()
            => _serviceScope?.Dispose();

        public void ResetState()
        {
            if (_serviceScope != null)
            {
                _serviceScope.Dispose();
                _serviceScope = null;
            }

            _applicationScopedServiceProvider = null;
        }

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            ResetState();
            return Task.CompletedTask;
        }
    }
}
