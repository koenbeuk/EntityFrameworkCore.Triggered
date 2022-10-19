using System;

namespace EntityFrameworkCore.Triggered
{
    public interface IAfterSaveFailedTrigger<TEntity>
        where TEntity : class
    {
        void AfterSaveFailed(ITriggerContext<TEntity> context, Exception exception);
    }


}
