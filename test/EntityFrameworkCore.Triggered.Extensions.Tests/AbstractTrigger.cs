using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Extensions.Tests
{
    public abstract class AbstractTrigger : IBeforeSaveTrigger<object>
    {
        public Task BeforeSave(ITriggerContext<object> context, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
