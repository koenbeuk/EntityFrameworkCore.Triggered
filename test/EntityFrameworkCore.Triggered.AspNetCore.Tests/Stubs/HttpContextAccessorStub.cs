using Microsoft.AspNetCore.Http;

namespace EntityFrameworkCore.Triggered.AspNetCore.Tests.Stubs
{
    public class HttpContextAccessorStub : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; } = new DefaultHttpContext();
    }
}
