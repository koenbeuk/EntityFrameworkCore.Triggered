using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EntityFrameworkCore.Triggered.AspNetCore.Tests.Stubs
{
    public class HttpContextAccessorStub : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; } = new DefaultHttpContext();
    }
}
