using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggers
{
    public interface IAfterSaveChangeEventHandler<TEntity>
        where TEntity: class
    {
        Task AfterSave(IChangeEvent<TEntity> @event, CancellationToken cancellationToken);
    }
}
