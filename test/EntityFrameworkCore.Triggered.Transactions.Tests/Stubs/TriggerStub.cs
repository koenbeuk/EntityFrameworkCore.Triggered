using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Transactions.Lifecycles;

namespace EntityFrameworkCore.Triggered.Transactions.Tests.Stubs
{
    public class TriggerStub<TEntity> : 
        IBeforeCommitTrigger<TEntity>,
        IBeforeCommitAsyncTrigger<TEntity>,
        IAfterCommitTrigger<TEntity>,
        IAfterCommitAsyncTrigger<TEntity>,
        IBeforeRollbackTrigger<TEntity>,
        IBeforeRollbackAsyncTrigger<TEntity>,
        IAfterRollbackTrigger<TEntity>,
        IAfterRollbackAsyncTrigger<TEntity>,
        IBeforeCommitStartingTrigger,
        IBeforeCommitStartingAsyncTrigger,
        IBeforeCommitCompletedTrigger,
        IBeforeCommitCompletedAsyncTrigger,
        IAfterCommitStartingTrigger,
        IAfterCommitStartingAsyncTrigger,
        IAfterCommitCompletedTrigger,
        IAfterCommitCompletedAsyncTrigger

        where TEntity : class
    {
        public int BeforeCommitStartingInvocationsCount { get; set; }
        public int BeforeCommitStartingAsyncInvocationsCount { get; set; }
        public int BeforeCommitCompletedInvocationsCount { get; set; }
        public int BeforeCommitCompletedAsyncInvocationsCount { get; set; }
        public int AfterCommitStartingInvocationsCount { get; set; }
        public int AfterCommitStartingAsyncInvocationsCount { get; set; }
        public int AfterCommitCompletedInvocationsCount { get; set; }
        public int AfterCommitCompletedAsyncInvocationsCount { get; set; }

        public ICollection<ITriggerContext<TEntity>> BeforeCommitInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> BeforeCommitAsyncInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterCommitInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterCommitAsyncInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> BeforeRollbackInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> BeforeRollbackAsyncInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterRollbackInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterRollbackAsyncInvocations { get; } = new List<ITriggerContext<TEntity>>();

        public void BeforeCommitStarting()
        {
            BeforeCommitStartingInvocationsCount++;
        }

        public Task BeforeCommitStartingAsync(CancellationToken cancellationToken)
        {
            BeforeCommitStartingAsyncInvocationsCount++;
            return Task.CompletedTask;
        }

        public void BeforeCommitCompleted()
        {
            BeforeCommitCompletedInvocationsCount++;
        }

        public Task BeforeCommitCompletedAsync(CancellationToken cancellationToken)
        {
            BeforeCommitCompletedAsyncInvocationsCount++;
            return Task.CompletedTask;
        }

        public void AfterCommitStarting()
        {
            AfterCommitStartingInvocationsCount++;
        }

        public Task AfterCommitStartingAsync(CancellationToken cancellationToken)
        {
            AfterCommitStartingAsyncInvocationsCount++;
            return Task.CompletedTask;
        }

        public void AfterCommitCompleted()
        {
            AfterCommitCompletedInvocationsCount++;
        }

        public Task AfterCommitCompletedAsync(CancellationToken cancellationToken)
        {
            AfterCommitCompletedAsyncInvocationsCount++;
            return Task.CompletedTask;
        }

        public void BeforeCommit(ITriggerContext<TEntity> context)
        {
            BeforeCommitInvocations.Add(context);
        }

        public Task BeforeCommitAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            BeforeCommitAsyncInvocations.Add(context);
            return Task.CompletedTask;
        }

        public void AfterCommit(ITriggerContext<TEntity> context)
        {
            AfterCommitInvocations.Add(context);
        }

        public Task AfterCommitAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            AfterCommitAsyncInvocations.Add(context);
            return Task.CompletedTask;
        }

        public void BeforeRollback(ITriggerContext<TEntity> context)
        {
            BeforeRollbackInvocations.Add(context);
        }

        public Task BeforeRollbackAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            BeforeRollbackAsyncInvocations.Add(context);
            return Task.CompletedTask;
        }

        public void AfterRollback(ITriggerContext<TEntity> context)
        {
            AfterRollbackInvocations.Add(context);
        }

        public Task AfterRollbackAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            AfterRollbackAsyncInvocations.Add(context);
            return Task.CompletedTask;
        }
    }
}
