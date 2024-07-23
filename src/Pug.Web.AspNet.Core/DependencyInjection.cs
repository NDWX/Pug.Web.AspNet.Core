using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Pug.Application;
using Pug.Application.Security;

namespace Pug.Web.AspNet.Core.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRequestSessionProvider(this IServiceCollection services)
        {
            services.AddSingleton<RequestSessionProvider>()
                .AddSingleton<IUserSessionProvider>(provider => provider.GetService<RequestSessionProvider>())
                .AddSingleton<IAspNetCoreSessionListener>(provider => provider.GetService<RequestSessionProvider>());

            return services;
        }

        public static IServiceCollection AddOAuth20IdentityAccessor(this IServiceCollection services)
        {
            //if( !services.Contains(ServiceDescriptor.Singleton(typeof(IHttpContextAccessor), typeof(HttpContextAccessor))))
            //    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddSingleton<SessionUserIdentityAccessor>();
            //services.AddSingleton<ISessionUserIdentityAccessor>(provider => provider.GetService<SessionUserIdentityAccessor>());
            services.AddSingleton<ISessionUserIdentityAccessor, OAuth20IdentityAccessor>();

            return services;
        }

        public static IServiceCollection AddSecurityManager(this IServiceCollection services,
                                                            string applicationName,
                                                            IUserRoleProvider userRoleProvider = null,
                                                            IAuthorizationProvider authorizationProvider = null)
        {
            services.AddSingleton<ISecurityManager>(
                    servicesProvider => new SecurityManager(
                            applicationName,
                            servicesProvider.GetService<ISessionUserIdentityAccessor>(),
                            userRoleProvider ?? servicesProvider.GetService<IUserRoleProvider>(),
                            authorizationProvider ?? servicesProvider.GetService<IAuthorizationProvider>()
                        )
                );

            return services;
        }

        public static IApplicationBuilder UseRequestSessionNotifier(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<RequestSessionNotifierMiddleware>();

            return builder;
        }

    }
}