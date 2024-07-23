using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Pug.Application.Security;

namespace Pug.Web.AspNet.Core
{
	//public abstract class SessionProvider : IUserSessionProvider, IAspNetCoreSessionListener
	//{
	//    IHttpContextAccessor httpContextAccessor;

	//    public SessionProvider(IHttpContextAccessor httpContextAccessor)
	//    {
	//        this.httpContextAccessor = httpContextAccessor;
	//    }

	//    public IHttpContextAccessor HttpContextAccessor => httpContextAccessor;

	//    public abstract IApplicationSession CurrentSession
	//    {
	//        get;
	//    }

	//    public abstract event SessionEventHandler SessionStarted;
	//    public abstract event SessionEventHandler SessionEnded;

	//    public abstract void OnNewSessionStarted(HttpContext context);

	//    public abstract void OnSessionEnded(HttpContext context);
	//}

	public class OAuth20IdentityAccessor : ISessionUserIdentityAccessor
	{
		public class AttributeNames
		{
			public static readonly string TokenIssuer = "token-issuer";
			public static readonly string TokenSubject = "token-subject";
			public static readonly string TokenAudience = "token-audience";
			public static readonly string TokenValidityStart = "token-validity-start";
			public static readonly string TokenValidityEnd = "token-validity-end";
			public static readonly string TokenIssueTimestamp = "token-issue-timestamp";
			public static readonly string TokenIdentifier = "token-identifier";
			public static readonly string IdentityServerScope = "identityserver-scope";
			public static readonly string IdentityServerClientIdentifier = "identityserver-client-identifier";
			public static readonly string AuthenticationType = "authentication-type";
		}

		private readonly IHttpContextAccessor httpContextAccessor;

		private readonly Dictionary<string, string> claimTypeDictionary = new Dictionary<string, string>()
		{
			["ISS"] = AttributeNames.TokenIssuer,
			["SUB"] = AttributeNames.TokenSubject,
			["AUD"] = AttributeNames.TokenAudience,
			["EXP"] = AttributeNames.TokenValidityEnd,
			["NBF"] = AttributeNames.TokenValidityStart,
			["IAT"] = AttributeNames.TokenIssueTimestamp,
			["JTI"] = AttributeNames.TokenIdentifier,
			["SCOPE"] = AttributeNames.IdentityServerScope,
			["CLIENT_ID"] = AttributeNames.IdentityServerClientIdentifier,
			["AMR"] = AttributeNames.AuthenticationType
		};

		public OAuth20IdentityAccessor(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		public IPrincipalIdentity GetUserIdentity()
		{
			HttpContext httpContext = httpContextAccessor.HttpContext;

			if (httpContext == null)
				return null;
			
			ClaimsPrincipal principal = httpContext.User;

			if (principal == null)
				return null;

			string client, subject, authenticationType;

			client = principal.Claims.Where(c => c.Type.ToUpper() == "CLIENT_ID")?.FirstOrDefault()?.Value;
			subject = principal.Claims.Where(c => c.Type.ToUpper() == "SUB")?.FirstOrDefault()?.Value;
			authenticationType = principal.Claims.Where(c => c.Type.ToUpper() == "AMR")?.FirstOrDefault()?.Value;

			Dictionary<string, string> attributes = new Dictionary<string, string>();

			string attributeName = string.Empty;

			foreach ( Claim claim in principal.Claims)
			{
				if (!claimTypeDictionary.ContainsKey(claim.Type.ToUpper()))
					continue;

				attributeName = $"oauth20.claims.{claimTypeDictionary[claim.Type.ToUpper()]}";

				if (attributes.ContainsKey(attributeName))
				{
					attributes[attributeName] = $"{attributes[attributeName]};{claim.Value}";
				}
				else
					attributes.Add(attributeName, claim.Value);
			}
			//ToDictionary(c => $"oauth20.claims.{c.Type}", c => c.Value);

			attributes.Add(IdentityAttributeNames.ClientIdentifier, client);

			return new BasicPrincipalIdentity(subject, subject, principal.Identity.IsAuthenticated, authenticationType, attributes);
		}
	}
}