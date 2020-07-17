using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Internal.RecursionStrategy;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests.Internal.RecursionStrategies
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
