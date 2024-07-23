using Microsoft.AspNetCore.Http;
using System;

namespace Pug.Web.AspNet.Core
{
    public abstract class Session : Application.IUserSession
    {
        private readonly HttpContext httpContext;

        public abstract event EventHandler Ending;

        public Session(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public HttpContext HttpContext => httpContext;

        public abstract T Get<T>(string identifier = "");

        public abstract void Set<T>(string identifier, T value);

        public abstract void Remove<T>(string identifier);
    }
}

