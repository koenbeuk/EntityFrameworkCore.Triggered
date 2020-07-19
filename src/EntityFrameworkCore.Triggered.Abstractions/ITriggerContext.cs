using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerContext<out TEntity>
        where TEntity: class
    {
        ChangeType ChangeType { get; }

        TEntity Entity { get; }

        TEntity? UnmodifiedEntity { get; }
    }
}
