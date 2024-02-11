using Microsoft.Extensions.Options;

namespace EntityFrameworkCore.Triggered.Tests.Stubs;

public class OptionsSnapshotStub<TOptions> : IOptionsSnapshot<TOptions>
    where TOptions : class, new()
{
    public TOptions Value => Activator.CreateInstance<TOptions>();

    public TOptions Get(string name) => Activator.CreateInstance<TOptions>();
}
