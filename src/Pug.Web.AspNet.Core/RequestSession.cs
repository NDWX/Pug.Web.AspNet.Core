using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace Pug.Web.AspNet.Core
{
    public class RequestSession : Session
    {
        public RequestSession(HttpContext httpContext) : base(httpContext)
        {
        }

        public override event EventHandler Ending;

        public override T Get<T>(string identifier = "")
        {
            if (!HttpContext.Items.ContainsKey(identifier))
                return default(T);

            Dictionary<Type, object> typeVariables = (Dictionary<Type, object>)HttpContext.Items[identifier];

            if (typeVariables.ContainsKey(typeof(T)))
                return (T)typeVariables[typeof(T)];

            return default(T);
        }

        public override void Remove<T>(string identifier)
        {
            Dictionary<Type, object> typeVariables;

            if (HttpContext.Items.ContainsKey(identifier))
            {
                typeVariables = (Dictionary<Type, object>)HttpContext.Items[identifier];

                typeVariables.Remove(typeof(T));

                if (typeVariables.Count == 0)
                    HttpContext.Items.Remove(identifier);
            }
        }

        public override void Set<T>(string identifier, T value)
        {
            Dictionary<Type, object> typeVariables;

            if (!HttpContext.Items.ContainsKey(identifier))
            {
                typeVariables = new Dictionary<Type, object>();
                HttpContext.Items.Add(identifier, typeVariables);
            }
            else
            {
                typeVariables = (Dictionary<Type, object>)HttpContext.Items[identifier];
            }

            typeVariables[typeof(T)] = value;
        }

        internal void NotifyEnding()
        {
            if (Ending != null)
                try
                {
                    Ending(this, EventArgs.Empty);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch(Exception)
                {
                }
        }
    }
}

