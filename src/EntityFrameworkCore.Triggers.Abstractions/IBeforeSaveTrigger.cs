using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggers
{
    public interface IBeforeSaveTrigger<in TEntity>
        where TEntity: class
    {
        Task BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
