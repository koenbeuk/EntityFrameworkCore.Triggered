﻿namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IBeforeCommitAsyncTrigger<in TEntity>
        where TEntity : class
    {
        Task BeforeCommitAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken);
    }
}
