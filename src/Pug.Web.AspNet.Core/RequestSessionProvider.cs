using System.Threading.Tasks;

using Pug.Application;

using Microsoft.AspNetCore.Http;

namespace Pug.Web.AspNet.Core
{
    public class RequestSessionProvider : IUserSessionProvider, IAspNetCoreSessionListener
    {
        private readonly object HTTP_CONTEXT_ITEM_KEY = typeof(IUserSessionProvider).FullName;
        private readonly IHttpContextAccessor httpContextAccessor;

        public RequestSessionProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public event SessionEventHandler SessionStarted = delegate { };
        public event SessionEventHandler SessionEnded = delegate { };

        private IUserSession CreateAndRegisterSession(HttpContext httpContext)
        {
            IUserSession session = new RequestSession(httpContext);

            httpContext.Items[HTTP_CONTEXT_ITEM_KEY] = session;

            return session;
        }

        public IUserSession CurrentSession
        {
            get
            {
                IUserSession session = null;

                HttpContext httpContext = httpContextAccessor.HttpContext;

                if (httpContext != null)
                    if (httpContext.Items.ContainsKey(HTTP_CONTEXT_ITEM_KEY))
                    {
                        session = (RequestSession)httpContext.Items[HTTP_CONTEXT_ITEM_KEY];
                    }
                    else
                    {
                        session = CreateAndRegisterSession(httpContext);
                    }

                return session;
            }
        }

        async Task IAspNetCoreSessionListener.OnSessionStartedAsync(HttpContext context)
        {
            IUserSession session = CreateAndRegisterSession(context);

            await Task.Run(() => SessionStarted(session));
        }

        async Task IAspNetCoreSessionListener.OnSessionEndedAsync(HttpContext context)
        {
            RequestSession session = (RequestSession)context.Items[HTTP_CONTEXT_ITEM_KEY];

            session.NotifyEnding();

            await Task.Run(() => SessionEnded(session));
        }
    }
}