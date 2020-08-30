using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;

namespace EntityFrameworkCore.Triggered.Tests.Internal.RecursionStrategies
{
    public class EntityAndTypeRecursionStrategyTests : RecursionStrategyTestsBase
    {
        protected override bool CanRecurseUnmodifiedExpectedOutcome => false;
        protected override bool CanRecurseModifiedExpectedOutcome => false;
        protected override bool CanRecurseUnmodifiedDifferentTypeExpectedOutcome => true;
        protected override bool CanRecurseModifiedDifferentTypeExpectedOutcome => true;

        protected override IRecursionStrategy CreateSubject()
            => new NoRecursionStrategy();
    }
}
