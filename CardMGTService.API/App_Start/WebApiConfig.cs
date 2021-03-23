using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Cors;
//using Swashbuckle.Application;
using Newtonsoft.Json;

namespace CardMGTService.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //config.EnableSwagger(c => c.SingleApiVersion("v1", "CardMGMTService"))
            //      .EnableSwaggerUi();

            EnableCrossSiteRequests(config);
            // Web API routes
            config.MapHttpAttributeRoutes();

          //  config.Routes.MapHttpRoute(
          //      name: "DefaultApi",
          //      routeTemplate: "{controller}/{id}",
          //      defaults: new { id = RouteParameter.Optional }
          //  );

          //  config.Routes.MapHttpRoute(
          //    name: "NamedActionApi",
          //    routeTemplate: "{controller}/{action}/{id}",
          //    defaults: new { id = RouteParameter.Optional }
          //);

          //  SetJsonSettings(config);
        }

        private static void EnableCrossSiteRequests(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*");
            config.EnableCors(cors);
        }

     
    }
}
