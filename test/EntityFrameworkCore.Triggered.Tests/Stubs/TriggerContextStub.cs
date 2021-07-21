using System.Collections.Generic;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerContextStub<TEntity> : ITriggerContext<TEntity>
        where TEntity : class

    {
        public ChangeType ChangeType { get; set; }
        public TEntity Entity { get; set; }
        public TEntity UnmodifiedEntity { get; set; }
        public IDictionary<object, object> EntityBag { get; set; }
    }
}
