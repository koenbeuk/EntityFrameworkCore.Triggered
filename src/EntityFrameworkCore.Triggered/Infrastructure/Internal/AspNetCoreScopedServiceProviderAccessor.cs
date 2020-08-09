using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Infrastructure.Internal
{
    public static class AspNetCoreScopedServiceProviderAccessor
    {
        public static readonly Func<IServiceProvider, IServiceProvider> AspNetCoreServiceProviderAccessor = sp => {

            return sp;
        };
    }
}
