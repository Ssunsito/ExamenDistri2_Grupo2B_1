using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ProyectoDistri2;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(ProyectoDistri2.WebAPI.Startup))]

namespace ProyectoDistri2.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configuración de autenticación OAuth
            var oAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true, // Solo desarrollo
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                Provider = new SimpleAuthorizationServerProvider()
            };

            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            // Configuración Web API
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }
    }
}
