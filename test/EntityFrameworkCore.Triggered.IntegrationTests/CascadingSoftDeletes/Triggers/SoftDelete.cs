using EntityFrameworkCore.Triggered.Extensions;
using EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes.Models;

namespace EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes.Triggers
{
    public class SoftDelete(ApplicationDbContext applicationDbContext) : Trigger<Branch>
    {
        readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

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
