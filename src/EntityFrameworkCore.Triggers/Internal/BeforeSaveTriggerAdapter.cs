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
    public sealed class BeforeSaveTriggerAdapter : TriggerAdapterBase
    {
        public BeforeSaveTriggerAdapter(object trigger) : base(trigger)
        {
        }

        public override Task Execute(object triggerContext, CancellationToken cancellationToken)
            => Execute(nameof(IBeforeSaveTrigger<object>.BeforeSave), triggerContext, cancellationToken);
    }
}
