using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Internal.RecursionStrategy;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests.Internal.RecursionStrategies
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
