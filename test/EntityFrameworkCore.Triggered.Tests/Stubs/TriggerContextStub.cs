using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerContextStub<TEntity> : ITriggerContext<TEntity>
        where TEntity : class

    {
        public ChangeType ChangeType { get; set; }
        public TEntity Entity { get; set; }
        public TEntity UnmodifiedEntity { get; set; }
    }
}
