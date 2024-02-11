using EntityFrameworkCore.Triggered.Extensions;
using EntityFrameworkCore.Triggered.IntegrationTests.SampleStore.Models;

namespace EntityFrameworkCore.Triggered.IntegrationTests.SampleStore.Triggers.Users
{
    public class SoftDeleteUsers(ApplicationDbContext applicationDbContext) : Trigger<User>
    {
        readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

        public override void BeforeSave(ITriggerContext<User> context)
        {
            if (context.ChangeType is ChangeType.Deleted)
            {
                _applicationDbContext.Entry(context.Entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.Entity.DeletedDate = DateTime.UtcNow;
            }
        }
    }
}
