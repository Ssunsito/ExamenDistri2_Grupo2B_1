/*
< !--*****************************************************
Proyecto Distri2 - Reserva.co
Archivo Startup.cs
Kenneth Pantoja
Santiago Pila
Fecha: 03 / 08 / 2025

// RESULTADOS FINALES
- Se implementó configuración de autenticación OAuth
- Se estableció sistema de tokens de acceso
- Se configuró tiempo de expiración de tokens
- Se integró middleware de autenticación
- Se centralizó la gestión de seguridad

// CONCLUSIONES
1. La configuración proporciona una base sólida para la autenticación
2. El sistema de tokens asegura la seguridad de las operaciones
3. La integración con Web API facilita la protección de endpoints
4. La configuración permite escalabilidad del sistema
5. El manejo de seguridad optimiza la protección de datos
*/

using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
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
