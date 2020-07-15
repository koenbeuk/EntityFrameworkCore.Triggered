using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.Triggers.Internal
{
    public sealed class AfterSaveEventHandlerAdapter : ChangeEventHandlerExecutionAdapterBase
    {
        public AfterSaveEventHandlerAdapter(object changeHandler) : base(changeHandler)
        {
        }

        public override Task Execute(object @event, CancellationToken cancellationToken)
            => Execute(nameof(IAfterSaveChangeEventHandler<object>.AfterSave), @event, cancellationToken);
    }
}
