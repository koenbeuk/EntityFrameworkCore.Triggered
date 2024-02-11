using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFrameworkCore.Triggered.Internal
{

    public interface ITriggerInstanceFactory
    {
        object Create(IServiceProvider serviceProvider);
    }

    public interface ITriggerInstanceFactory<out TTriggerType> : ITriggerInstanceFactory, IResettableService
    {

    }

    public sealed class TriggerInstanceFactory<TTriggerType>(object? instance) : ITriggerInstanceFactory<TTriggerType>
    {
        static ObjectFactory? _internalFactory;

        object? _instance = instance;

        public object Create(IServiceProvider serviceProvider)
        {
            if (_instance is not null)
            {
                return _instance;
            }

            _internalFactory ??= ActivatorUtilities.CreateFactory(typeof(TTriggerType), []);

            _instance = _internalFactory(serviceProvider, null);
            return _instance;
        }

        public void ResetState() => _instance = null;

        public Task ResetStateAsync(CancellationToken cancellationToken = default)
        {
            ResetState();
            return Task.CompletedTask;
        }
    }
}
