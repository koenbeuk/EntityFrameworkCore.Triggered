using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Internal;

namespace EntityFrameworkCore.Triggers.Tests.Stubs
{
    [ExcludeFromCodeCoverage]
    public class ChangeEventHandlerEventExecutionStrategyStub : ChangeEventHandlerExecutionAdapterBase
    {
        public ChangeEventHandlerEventExecutionStrategyStub(object changeHandler) : base(changeHandler)
        {
        }

        public override Task Execute(object @event, CancellationToken cancellationToken)
        {
            Executions.Add(@event);
            return Task.CompletedTask;
        }

        public List<object> Executions { get; } = new List<object>();
    }
}
