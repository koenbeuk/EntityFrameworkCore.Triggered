using System;

namespace EntityFrameworkCore.Triggers.Internal
{
    public interface ITriggerRegistryService
    {
        TriggerRegistry GetRegistry(Type changeHandlerType, Func<object, TriggerAdapterBase> executionStrategyFactory);
    }
}