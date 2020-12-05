using EntityFrameworkCore.Triggered.Internal.CascadeStrategies;

namespace EntityFrameworkCore.Triggered.Tests.Internal.CascadeStrategies
{
    public class NoCascadeStrategyTests : CascadeStrategyTestsBase
    {
        protected override bool CanCascadeUnmodifiedExpectedOutcome => false;
        protected override bool CanCascadeModifiedExpectedOutcome => false;
        protected override bool CanCascadeUnmodifiedDifferentTypeExpectedOutcome => false;
        protected override bool CanCascadeModifiedDifferentTypeExpectedOutcome => false;

        protected override ICascadeStrategy CreateSubject()
            => new NoCascadeStrategy();
    }
}
