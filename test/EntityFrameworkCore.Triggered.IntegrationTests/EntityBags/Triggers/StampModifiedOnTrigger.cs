namespace EntityFrameworkCore.Triggered.IntegrationTests.EntityBags.Triggers
{
    public class StampModifiedOnTrigger : IBeforeSaveTrigger<User>
    {
        public void BeforeSave(ITriggerContext<User> context)
        {
            if (context.ChangeType is ChangeType.Modified)
            {
                if (!context.Items.ContainsKey(SoftDeleteTrigger.IsSoftDeleted))
                {
                    context.Entity.ModifiedOn = DateTime.UtcNow;
                }
            }
        }
    }
}
