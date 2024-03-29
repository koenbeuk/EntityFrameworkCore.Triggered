﻿using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IBeforeRollbackAsyncTrigger<in TEntity>
        where TEntity : class
    {
        Task BeforeRollbackAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
