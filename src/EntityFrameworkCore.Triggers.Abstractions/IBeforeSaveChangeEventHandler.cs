using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggers
{
    public interface IBeforeSaveChangeEventHandler<in TEntity>
        where TEntity: class
    {
        Task BeforeSave(IChangeEvent<TEntity> @event, CancellationToken cancellationToken);
    }
}
