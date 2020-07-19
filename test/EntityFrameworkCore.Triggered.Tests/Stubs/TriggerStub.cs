using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    [ExcludeFromCodeCoverage]
    public class TriggerStub<TEntity> : IBeforeSaveTrigger<TEntity>, IAfterSaveTrigger<TEntity>
        where TEntity: class
    {
        public ICollection<ITriggerContext<TEntity>> BeforeSaveInvocations { get; } = new List<ITriggerContext<TEntity>>();
        public ICollection<ITriggerContext<TEntity>> AfterSaveInvocations { get; } = new List<ITriggerContext<TEntity>>();

        public TriggerStub()
        {

        }

        public Task BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            BeforeSaveInvocations.Add(context);
            return Task.CompletedTask;
        }

        public Task AfterSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken)
        {
            AfterSaveInvocations.Add(context);
            return Task.CompletedTask;
        }
    }
}
