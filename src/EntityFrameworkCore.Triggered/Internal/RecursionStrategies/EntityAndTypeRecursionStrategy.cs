using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkCore.Triggered.Internal.RecursionStrategy
{
    public class EntityAndTypeRecursionStrategy : IRecursionStrategy
    {
        public bool CanRecurse(EntityEntry entry, ChangeType changeType, ITriggerContextDescriptor previousTriggerContextDescriptor) 
            => changeType != previousTriggerContextDescriptor.Type;
    }
}
