using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Pug.Web.AspNet.Core
{
    public class RequestSessionNotifierMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestSessionNotifierMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAspNetCoreSessionListener sessionProvider)
        {
            await sessionProvider.OnSessionStartedAsync(context);

            // Call the next delegate/middleware in the pipeline
            await _next(context);

            await sessionProvider.OnSessionEndedAsync(context);
        }
    }
}

