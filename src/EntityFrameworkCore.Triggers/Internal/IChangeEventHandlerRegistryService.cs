using System;

namespace EntityFrameworkCore.Triggers.Internal
{
    public interface IChangeEventHandlerRegistryService
    {
        ChangeEventHandlerRegistry GetRegistry(Type changeHandlerType, Func<object, ChangeEventHandlerExecutionAdapterBase> executionStrategyFactory);
    }
}