using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Transactions.Tests.Stubs
{
    public class TriggerStub<TEntity> : IBeforeCommitTrigger<TEntity>, IAfterCommitTrigger<TEntity>, IBeforeRollbackTrigger<TEntity>, IAfterRollbackTrigger<TEntity>
        where TEntity : class
    {
        public ICollection<ITriggerContext<TEntity>> BeforeCommitInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterCommitInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> BeforeRollbackInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterRollbackInvocations { get; } = new List<ITriggerContext<TEntity>>();


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
