using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerSession
    {
        Task RaiseBeforeSaveTriggers(CancellationToken cancellationToken = default);
        Task RaiseAfterSaveTriggers(CancellationToken cancellationToken = default);
    }
}
