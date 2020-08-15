using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class TriggerDescriptorComparer : IComparer<TriggerDescriptor>
    {
        public int Compare(TriggerDescriptor x, TriggerDescriptor y)
        {
            if (x is null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y is null)
            {
                throw new ArgumentNullException(nameof(y));
            }

            return x.Priority - y.Priority;
        }
    }
}
