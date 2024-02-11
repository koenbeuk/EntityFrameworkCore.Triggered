using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerStub<TEntity> :
        IBeforeSaveTrigger<TEntity>,
        IBeforeSaveAsyncTrigger<TEntity>,
        IAfterSaveTrigger<TEntity>,
        IAfterSaveAsyncTrigger<TEntity>,
        IAfterSaveFailedTrigger<TEntity>,
        IAfterSaveFailedAsyncTrigger<TEntity>,
        ITriggerPriority
        where TEntity : class
    {
        public ICollection<ITriggerContext<TEntity>> BeforeSaveInvocations { get; } = [];
        public ICollection<ITriggerContext<TEntity>> BeforeSaveAsyncInvocations { get; } = [];
        public ICollection<ITriggerContext<TEntity>> AfterSaveInvocations { get; } = [];
        public ICollection<ITriggerContext<TEntity>> AfterSaveAsyncInvocations { get; } = [];
        public ICollection<(ITriggerContext<TEntity> context, Exception exception)> AfterSaveFailedInvocations { get; } = new List<(ITriggerContext<TEntity>, Exception)>();
        public ICollection<(ITriggerContext<TEntity> context, Exception exception)> AfterSaveFailedAsyncInvocations { get; } = new List<(ITriggerContext<TEntity>, Exception)>();

        public int Priority { get; set; }

        public Action<ITriggerContext<TEntity>> BeforeSaveHandler { get; set; }
        public Func<ITriggerContext<TEntity>, CancellationToken, Task> BeforeSaveAsyncHandler { get; set; }
        public Action<ITriggerContext<TEntity>> AfterSaveHandler { get; set; }
        public Func<ITriggerContext<TEntity>, CancellationToken, Task> AfterSaveAsyncHandler { get; set; }
        public Action<ITriggerContext<TEntity>, Exception> AfterSaveFailedHandler { get; set; }
        public Func<ITriggerContext<TEntity>, Exception, CancellationToken, Task> AfterSaveFailedAsyncHandler { get; set; }

        public TriggerStub()
        {

        }

        public void BeforeSave(ITriggerContext<TEntity> context)
        {
            BeforeSaveInvocations.Add(context);
            BeforeSaveHandler?.Invoke(context);
        }

        public Task BeforeSaveAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            BeforeSaveAsyncInvocations.Add(context);
            BeforeSaveAsyncHandler?.Invoke(context, cancellationToken);
            return Task.CompletedTask;
        }

        public void AfterSave(ITriggerContext<TEntity> context)
        {
            AfterSaveInvocations.Add(context);
            AfterSaveHandler?.Invoke(context);
        }

        public Task AfterSaveAsync(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            AfterSaveAsyncInvocations.Add(context);
            AfterSaveAsyncHandler?.Invoke(context, cancellationToken);
            return Task.CompletedTask;
        }


        public void AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception)
        {
            AfterSaveFailedInvocations.Add((context, exception));
            AfterSaveFailedHandler?.Invoke(context, exception);
        }

        public Task AfterSaveFailedAsync(ITriggerContext<TEntity> context, Exception exception, CancellationToken cancellationToken)
        {
            AfterSaveFailedAsyncInvocations.Add((context, exception));
            AfterSaveFailedAsyncHandler?.Invoke(context, exception, cancellationToken);
            return Task.CompletedTask;
        }
    }
}
