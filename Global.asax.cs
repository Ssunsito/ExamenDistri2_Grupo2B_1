using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProyectoDistri2
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var context = HttpContext.Current;
            var path = context.Request.Path.ToLower();

            // Permitir rutas públicas
            if (path.Contains("/account/login") || path.StartsWith("/api/")) return;

            if (context.Session != null && context.Session["Token"] == null)
            {
                context.Response.Redirect("/Account/Login");
            }
        }

    }
}
