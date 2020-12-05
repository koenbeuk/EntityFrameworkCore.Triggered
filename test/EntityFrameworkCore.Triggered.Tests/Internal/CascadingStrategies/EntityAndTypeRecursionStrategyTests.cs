using EntityFrameworkCore.Triggered.Internal.CascadingStrategies;

namespace EntityFrameworkCore.Triggered.Tests.Internal.CascadingStrategies
{
    public class EntityAndTypeCascadingStrategyTests : CascadingStrategyTestsBase
    {
        protected override bool CanCascadeUnmodifiedExpectedOutcome => false;
        protected override bool CanCascadeModifiedExpectedOutcome => false;
        protected override bool CanCascadeUnmodifiedDifferentTypeExpectedOutcome => true;
        protected override bool CanCascadeModifiedDifferentTypeExpectedOutcome => true;

        protected override ICascadingStrategy CreateSubject()
            => new NoCascadingStrategy();
    }
}
