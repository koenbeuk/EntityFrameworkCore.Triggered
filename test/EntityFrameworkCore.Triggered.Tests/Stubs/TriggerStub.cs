using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerStub<TEntity> : IBeforeSaveTrigger<TEntity>, IAfterSaveTrigger<TEntity>, IAfterSaveFailedTrigger<TEntity>, ITriggerPriority
        where TEntity: class
    {
        public ICollection<ITriggerContext<TEntity>> BeforeSaveInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterSaveInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<(ITriggerContext<TEntity> context, Exception exception)> AfterSaveFailedInvocations { get; } = new List<(ITriggerContext<TEntity>, Exception)>();

        public int Priority { get; set; }

        public Func<ITriggerContext<TEntity>, CancellationToken, Task> BeforeSaveHandler { get; set; }
        public Func<ITriggerContext<TEntity>, CancellationToken, Task> AfterSaveHandler { get; set; }
        public Func<ITriggerContext<TEntity>, Exception, CancellationToken, Task> AfterSaveFailedHandler { get; set; }

        public TriggerStub()
        {

        }

        public Task BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            BeforeSaveHandler?.Invoke(context, cancellationToken);

            BeforeSaveInvocations.Add(context);
            return Task.CompletedTask;
        }

        public Task AfterSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            AfterSaveHandler?.Invoke(context, cancellationToken);

            AfterSaveInvocations.Add(context);
            return Task.CompletedTask;
        }

        public Task AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken)
        {
            AfterSaveFailedHandler?.Invoke(context, exception, cancellationToken);
            AfterSaveFailedInvocations.Add((context, exception));
            return Task.CompletedTask;
        }
    }
}
