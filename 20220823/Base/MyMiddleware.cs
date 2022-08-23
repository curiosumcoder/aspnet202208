using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using System;

namespace WA11
{
    public class MyMiddleware
    {

        private readonly RequestDelegate _next;
        public MyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {            
            context.Response.Headers.Add("X-MyMiddleware", 
                new StringValues($"Welcome at {DateTime.Now.ToString()}!"));
            await _next.Invoke(context);
        }
    }
}
