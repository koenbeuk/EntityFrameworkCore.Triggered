using EntityFrameworkCore.Triggered.Internal.RecursionStrategy;

namespace EntityFrameworkCore.Triggered.Tests.Internal.RecursionStrategies
{
    public class NoRecursionStrategyTests : RecursionStrategyTestsBase
    {
        protected override bool CanRecurseUnmodifiedExpectedOutcome => false;
        protected override bool CanRecurseModifiedExpectedOutcome => false;
        protected override bool CanRecurseUnmodifiedDifferentTypeExpectedOutcome => false;
        protected override bool CanRecurseModifiedDifferentTypeExpectedOutcome => false;

        protected override IRecursionStrategy CreateSubject()
            => new NoRecursionStrategy();
    }
}
