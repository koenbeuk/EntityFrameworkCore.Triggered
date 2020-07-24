using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerContextFactoryTests
    {
        [Fact]
        public void Activate_ReturnsInstance()
        {
            var result = TriggerContextFactory<object>.Activate(null, ChangeType.Added);

            Assert.NotNull(result);
        }

    }
}
