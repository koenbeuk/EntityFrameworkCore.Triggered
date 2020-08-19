using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TriggersContextOptionsBuilderExtensions
    {
        public static TriggersContextOptionsBuilder UseAspNetCoreIntegration(this TriggersContextOptionsBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.UseApplicationScopedServiceProviderAccessor(serviceProvider => {
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                if (httpContextAccessor == null)
                {
                    throw new InvalidOperationException("No service for type 'Microsoft.AspNetCore.Http.IHttpContextAccessor' has been registered. Please make sure to call 'services.AddHttpContextAccessor()'");
                }
                return httpContextAccessor.HttpContext.RequestServices; 
            });

            return builder;
        }
    }
}
