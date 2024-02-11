using System;

namespace EntityFrameworkCore.Triggered.IntegrationTests.EntityBags.Triggers
{
    public class SoftDeleteTrigger(ApplicationDbContext dbContext) : IBeforeSaveTrigger<User>
    {
        public const string IsSoftDeleted = "SoftDeleteTrigger_IsSoftDeleted";

        readonly ApplicationDbContext _dbContext = dbContext;

        public void BeforeSave(ITriggerContext<User> context)
        {
            if (context.ChangeType is ChangeType.Deleted)
            {
                context.Items[IsSoftDeleted] = true;
                context.Entity.DeletedOn = DateTime.UtcNow;

                _dbContext.Entry(context.Entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
        }
    }
}
