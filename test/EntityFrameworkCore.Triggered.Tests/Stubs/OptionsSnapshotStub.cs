using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class OptionsSnapshotStub<TOptions> : IOptionsSnapshot<TOptions>
        where TOptions : class
    {
        public TOptions Value => Activator.CreateInstance<TOptions>();

        public TOptions Get(string name) => Activator.CreateInstance<TOptions>();
    }
}
