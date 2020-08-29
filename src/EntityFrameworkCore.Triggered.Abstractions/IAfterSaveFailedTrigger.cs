using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface IAfterSaveFailedTrigger<TEntity>
        where TEntity : class
    {
        Task AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken);
    }
}
