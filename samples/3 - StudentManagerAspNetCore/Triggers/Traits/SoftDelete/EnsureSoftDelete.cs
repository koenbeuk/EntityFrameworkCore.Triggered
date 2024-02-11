using EntityFrameworkCore.Triggered;
using StudentManager.Traits;

namespace StudentManager.Triggers.Traits.SoftDelete;

class EnsureSoftDelete(ApplicationDbContext applicationContext) : IBeforeSaveTrigger<ISoftDelete>
{
    readonly ApplicationDbContext _applicationContext = applicationContext;

    public void BeforeSave(ITriggerContext<ISoftDelete> context)
    {
        if (context.ChangeType == ChangeType.Deleted)
        {
            context.Entity.DeletedOn = DateTimeOffset.Now;
            _applicationContext.Entry(context.Entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
