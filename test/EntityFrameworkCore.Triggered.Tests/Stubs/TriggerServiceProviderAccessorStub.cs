using System;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerServiceProviderAccessorStub(IServiceProvider serviceProvider) : ITriggerServiceProviderAccessor
    {
        readonly IServiceProvider _serviceProvider = serviceProvider;

        public IServiceProvider GetTriggerServiceProvider() => _serviceProvider;
    }
}
