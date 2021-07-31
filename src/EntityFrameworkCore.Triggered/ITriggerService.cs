using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerService
    {
        TriggerConfiguration Configuration { get; set; }

        ITriggerSession CreateSession(DbContext context, IServiceProvider? serviceProvider = null);

        ITriggerSession? Current { get; set; }
    }
}
