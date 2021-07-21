using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.IntegrationTests.EntityBags.Triggers
{
    public class SoftDeleteTrigger : IBeforeSaveTrigger<User>
    {
        public const string IsSoftDeleted = "SoftDeleteTrigger_IsSoftDeleted";

        readonly ApplicationDbContext _dbContext;

        public SoftDeleteTrigger(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task BeforeSave(ITriggerContext<User> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType is ChangeType.Deleted)
            {
                context.EntityBag[IsSoftDeleted] = true;
                context.Entity.DeletedOn = DateTime.UtcNow;

                _dbContext.Entry(context.Entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }

            return Task.CompletedTask;
        }
    }
}
