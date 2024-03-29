﻿using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface IBeforeSaveAsyncTrigger<in TEntity>
        where TEntity : class
    {
        Task BeforeSaveAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
