using EntityFrameworkCore.Triggered.Internal.CascadeStrategies;

namespace EntityFrameworkCore.Triggered.Tests.Internal.CascadingStrategies;

public class NoCascadingStrategyTests : CascadingStrategyTestsBase
{
    protected override bool CanCascadeUnmodifiedExpectedOutcome => false;
    protected override bool CanCascadeModifiedExpectedOutcome => false;
    protected override bool CanCascadeUnmodifiedDifferentTypeExpectedOutcome => false;
    protected override bool CanCascadeModifiedDifferentTypeExpectedOutcome => false;

    protected override ICascadeStrategy CreateSubject()
        => new NoCascadeStrategy();
}
