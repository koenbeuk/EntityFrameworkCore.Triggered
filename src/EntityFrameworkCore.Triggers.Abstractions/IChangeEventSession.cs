using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggers
{
    public interface IChangeEventSession
    {
        Task RaiseBeforeSaveChangeEvents(CancellationToken cancellationToken = default);
        Task RaiseAfterSaveChangeEvents(CancellationToken cancellationToken = default);
    }
}
