using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Pug.Web.AspNet.Core
{
    public interface IAspNetCoreSessionListener
    {
        Task OnSessionStartedAsync(HttpContext context);

        Task OnSessionEndedAsync(HttpContext context);
    }
}

