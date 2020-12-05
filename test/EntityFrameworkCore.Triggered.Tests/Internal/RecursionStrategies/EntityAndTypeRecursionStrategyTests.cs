using EntityFrameworkCore.Triggered.Internal.CascadeStrategies;

namespace EntityFrameworkCore.Triggered.Tests.Internal.CascadeStrategies
{
    public class EntityAndTypeCascadeStrategyTests : CascadeStrategyTestsBase
    {
        protected override bool CanCascadeUnmodifiedExpectedOutcome => false;
        protected override bool CanCascadeModifiedExpectedOutcome => false;
        protected override bool CanCascadeUnmodifiedDifferentTypeExpectedOutcome => true;
        protected override bool CanCascadeModifiedDifferentTypeExpectedOutcome => true;

        protected override ICascadeStrategy CreateSubject()
            => new NoCascadeStrategy();
    }
}
