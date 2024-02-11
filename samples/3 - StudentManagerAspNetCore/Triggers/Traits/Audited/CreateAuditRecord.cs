using System.Text;
using EntityFrameworkCore.Triggered;
using StudentManager.Traits;

namespace StudentManager.Triggers.Traits.Audited;

public class CreateAuditRecord(ApplicationDbContext applicationContext) : IBeforeSaveTrigger<IAudited>
{
    private readonly ApplicationDbContext _applicationContext = applicationContext;

    public void BeforeSave(ITriggerContext<IAudited> context)
    {
        var recordBuilder = new StringBuilder();

        var changes = context.Entity.GetType()
            .GetProperties()
            .Select(property => (name: property.Name, oldValue: context.ChangeType != ChangeType.Added ? property.GetValue(context.UnmodifiedEntity) : null, newValue: property.GetValue(context.Entity)))
            .Where(x => context.ChangeType == ChangeType.Added || x.newValue != x.oldValue);

        foreach (var change in changes)
        {
            recordBuilder.AppendLine($"{change.name}: {change.oldValue ?? "null"} => {change.newValue ?? "null"}");
        }

        _applicationContext.Audits.Add(new Audit {
            Id = context.Entity.Id,
            Discriminator = context.Entity.GetType().Name,
            RecordDate = DateTimeOffset.Now,
            Record = recordBuilder.ToString()
        });
    }
}
