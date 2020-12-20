using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Transactions.Abstractions.Lifecycles;

namespace EntityFrameworkCore.Triggered.Transactions.Tests.Stubs
{
    public class TriggerStub<TEntity> : IBeforeCommitTrigger<TEntity>, IAfterCommitTrigger<TEntity>, IBeforeRollbackTrigger<TEntity>, IAfterRollbackTrigger<TEntity>,
        IBeforeCommitStartingTrigger, IBeforeCommitStartedTrigger, IAfterCommitStartingTrigger, IAfterCommitStartedTrigger
        where TEntity : class
    {
        public int BeforeCommitStartingInvocationsCount { get; set; }
        public int BeforeCommitStartedInvocationsCount { get; set; }
        public int AfterCommitStartingInvocationsCount { get; set; }
        public int AfterCommitStartedInvocationsCount { get; set; }

        public ICollection<ITriggerContext<TEntity>> BeforeCommitInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterCommitInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> BeforeRollbackInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterRollbackInvocations { get; } = new List<ITriggerContext<TEntity>>();

        public Task BeforeCommitStarting(CancellationToken cancellationToken)
        {
            BeforeCommitStartingInvocationsCount++;
            return Task.CompletedTask;
        }

        public Task BeforeCommitStarted(CancellationToken cancellationToken)
        {
            BeforeCommitStartedInvocationsCount++;
            return Task.CompletedTask;
        }

        public Task AfterCommitStarting(CancellationToken cancellationToken)
        {
            AfterCommitStartingInvocationsCount++;
            return Task.CompletedTask;
        }

        public Task AfterCommitStarted(CancellationToken cancellationToken)
        {
            AfterCommitStartedInvocationsCount++;
            return Task.CompletedTask;
        }

        public Task BeforeCommit(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            BeforeCommitInvocations.Add(context);
            return Task.CompletedTask;
        }

        public Task AfterCommit(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            AfterCommitInvocations.Add(context);
            return Task.CompletedTask;
        }

        public Task BeforeRollback(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            BeforeRollbackInvocations.Add(context);
            return Task.CompletedTask;
        }

        public Task AfterRollback(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            AfterRollbackInvocations.Add(context);
            return Task.CompletedTask;
        }
    }
}
