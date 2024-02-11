﻿namespace EntityFrameworkCore.Triggered.Transactions
{
    public interface IBeforeCommitTrigger<in TEntity>
        where TEntity : class
    {
        void BeforeCommit(ITriggerContext<TEntity> context);
    }
}
