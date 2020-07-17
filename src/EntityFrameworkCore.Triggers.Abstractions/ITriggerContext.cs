using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggers
{
    public interface ITriggerContext<out TEntity>
        where TEntity: class
    {
        ChangeType Type { get; }

        TEntity Entity { get; }

        TEntity? UnmodifiedEntity { get; }
    }
}
