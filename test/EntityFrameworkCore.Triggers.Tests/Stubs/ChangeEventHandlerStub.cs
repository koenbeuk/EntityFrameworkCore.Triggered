using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggers.Tests.Stubs
{
    [ExcludeFromCodeCoverage]
    public class ChangeEventHandlerStub<TEntity> : ChangeEventHandler<TEntity>
        where TEntity: class
    {
        public ICollection<IChangeEvent<TEntity>> BeforeSaveInvocations { get; } = new List<IChangeEvent<TEntity>>();
        public ICollection<IChangeEvent<TEntity>> AfterSaveInvocations { get; } = new List<IChangeEvent<TEntity>>();

        public ChangeEventHandlerStub()
        {

        }

        public override Task BeforeSave(IChangeEvent<TEntity> @event, CancellationToken cancellationToken)
        {
            BeforeSaveInvocations.Add(@event);
            return Task.CompletedTask;
        }

        public override Task AfterSave(IChangeEvent<TEntity> @event, CancellationToken cancellationToken)
        {
            AfterSaveInvocations.Add(@event);
            return Task.CompletedTask;
        }
    }
}
