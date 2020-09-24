using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class ApplicationTriggerServiceProviderAccessor : ITriggerServiceProviderAccessor, IDisposable, IResettableService
    {
        readonly IServiceProvider _rootServiceProvider;
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

            _rootServiceProvider = coreOptionsExtension.ApplicationServiceProvider ?? internalServiceProvider;
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
                if (_scopedServiceProviderTransform == null)
                {
                    _serviceScope = _rootServiceProvider.CreateScope();
                    _applicationScopedServiceProvider = _serviceScope.ServiceProvider;
                }
                else
                {
                    _applicationScopedServiceProvider = _scopedServiceProviderTransform(_rootServiceProvider);
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
