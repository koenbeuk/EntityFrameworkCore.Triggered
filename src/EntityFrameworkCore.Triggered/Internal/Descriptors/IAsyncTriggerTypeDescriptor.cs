﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal.Descriptors
{
    public interface IAsyncTriggerTypeDescriptor
    {
        Type TriggerType { get; }
        Task Invoke(object trigger, object triggerContext, Exception? exception, CancellationToken cancellationToken);
    }

}
