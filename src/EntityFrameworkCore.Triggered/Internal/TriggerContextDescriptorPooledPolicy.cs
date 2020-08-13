using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerContextDescriptorPooledPolicy : PooledObjectPolicy<TriggerContextDescriptor>
    {
        public override TriggerContextDescriptor Create() => new TriggerContextDescriptor();

        public override bool Return(TriggerContextDescriptor obj)
        {
            obj.Reset();
            return true;
        }
    }
}
