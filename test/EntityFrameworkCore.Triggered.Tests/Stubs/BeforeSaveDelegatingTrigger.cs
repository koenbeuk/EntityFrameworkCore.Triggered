﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class BeforeSaveDelegatingTrigger<TEntity> : IBeforeSaveTrigger<TEntity>
        where TEntity : class
    {
        readonly Func<ITriggerContext<TEntity>, CancellationToken, Task> _handler;

        public BeforeSaveDelegatingTrigger(Func<ITriggerContext<TEntity>, CancellationToken, Task> handler)
        {
            _handler = handler;
        }

        public Task BeforeSave(ITriggerContext<TEntity> context, CancellationToken cancellationToken) => _handler(context, cancellationToken);
    }
}
