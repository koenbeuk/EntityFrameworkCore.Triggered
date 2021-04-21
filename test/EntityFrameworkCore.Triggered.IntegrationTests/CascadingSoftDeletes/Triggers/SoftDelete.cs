using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Extensions;
using EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes.Models;

namespace EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes.Triggers
{
    public class SoftDelete : Trigger<Branch>
    {
        readonly ApplicationDbContext _applicationDbContext;

        public SoftDelete(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public override void BeforeSave(ITriggerContext<Branch> context)
        {
            if (context.ChangeType is ChangeType.Deleted)
            {
                _applicationDbContext.Entry(context.Entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.Entity.DeletedOn = DateTime.UtcNow;
            }
        }
    }
}
