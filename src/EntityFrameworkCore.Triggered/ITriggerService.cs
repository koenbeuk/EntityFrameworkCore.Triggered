using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerService
    {
        TriggerSessionConfiguration Configuration { get; set; }

        ITriggerSession CreateSession(DbContext context, IServiceProvider? serviceProvider = null);

        ITriggerSession CreateSession(DbContext context, TriggerSessionConfiguration configuration, IServiceProvider? serviceProvider = null);

        ITriggerSession? Current { get; set; }
    }
}
