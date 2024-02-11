﻿namespace EntityFrameworkCore.Triggered.Transactions;

public interface IAfterCommitTrigger<in TEntity>
    where TEntity : class
{
    void AfterCommit(ITriggerContext<TEntity> context);
}
