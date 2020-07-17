using System;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerRegistryService
    {
        TriggerRegistry GetRegistry(Type changeHandlerType, Func<object, TriggerAdapterBase> executionStrategyFactory);
    }
}