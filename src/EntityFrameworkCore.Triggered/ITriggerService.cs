using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerService
    {
        ITriggerSession CreateSession(DbContext context, IServiceProvider? serviceProvider = null);

        ITriggerSession? Current { get; set; }
    }
}
