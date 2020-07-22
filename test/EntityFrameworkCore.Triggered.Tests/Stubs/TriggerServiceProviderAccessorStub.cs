using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerServiceProviderAccessorStub : ITriggerServiceProviderAccessor
    {
        readonly IServiceProvider _serviceProvider;

        public TriggerServiceProviderAccessorStub(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public IServiceProvider GetTriggerServiceProvider()
        {
            return _serviceProvider;
        }
    }
}
