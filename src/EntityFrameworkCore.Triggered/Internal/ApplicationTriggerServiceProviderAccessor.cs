using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

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
            var coreOptionsExtension = dbContextOptions.FindExtension<CoreOptionsExtension>() ?? throw new InvalidOperationException("No coreOptionsExtension configured");

            _internalServiceProvider = internalServiceProvider;
            if (scopedServiceProviderTransform == null)
            {
                _fallbackApplicationServiceProvider = coreOptionsExtension.ApplicationServiceProvider;
            }

            _scopedServiceProviderTransform = scopedServiceProviderTransform;
        }

        public void SetTriggerServiceProvider(IServiceProvider serviceProvider)
        {
            if (_applicationScopedServiceProvider is not null && _applicationScopedServiceProvider != serviceProvider)
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
                    var serviceProvider = coreOptionsExtension!.ApplicationServiceProvider ?? _internalServiceProvider;

                    _applicationScopedServiceProvider = _scopedServiceProviderTransform(serviceProvider);
                }
                else
                {
                    var dbContext = _internalServiceProvider.GetRequiredService<ICurrentDbContext>().Context;
                    _applicationScopedServiceProvider = new HybridServiceProvider(_internalServiceProvider, dbContext);
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
