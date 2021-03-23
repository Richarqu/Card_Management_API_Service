using Microsoft.ApplicationInsights;
using CardMGTService.Core.Domain.Model;
using CardMGTService.Core.Persistence.Cache;
using CardMGTService.Core.Persistence.EF;
using CardMGTService.Core.Repositories;
using CardMGTService.Core.Services.Impl;
using CardMGTService.Core.Services.Interfaces;
using CardMGTService.Core.Util;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using SimpleInjector.Integration.WebApi;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CardMGTService.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);



            //var container = new Container();
            //container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();

            //// Register your types, for instance using the scoped lifestyle:
            ////container.RegisterConditional(typeof(IGenericRepository<,>),
            ////   typeof(GenericRepository<,>), Lifestyle.Scoped, (c) => !c.Handled);

            //container.Register(() => new UnitedSecurityContext(), Lifestyle.Scoped);
            //RegisterDependencies(container);

            //// This is an extension method from the integration package.
            //container.RegisterWebApiControllers(GlobalConfiguration.Configuration);
            ////container.RegisterSingleton(container);
            ////container.RegisterSingleton()
            ////container.Register<ICache, Cache>(Lifestyle.Singleton);


            //container.Verify();

            //GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            // AutoMapperConfig.RegisterMappings();
            //SystemInformation.OnApplicationStart();
            //GlobalConfiguration.Configure(RegisterWebApi);
        }
        private void RegisterDependencies(Container container)
        {
            container.Register<IUnitOfWork, UnitOfWork>(Lifestyle.Scoped);
         
            container.Register<IMailer, Mailer>(Lifestyle.Scoped);
   
            
            container.Register<ICacheManager, CacheManager>(Lifestyle.Scoped);
     
            container.Register<IServiceHelper, ServiceHelper>(Lifestyle.Scoped);
            container.Register<IUserManagerService<User>, UserManagerService>(Lifestyle.Scoped);
          
        }



        //protected void Application_Start()
        //{
        //    AreaRegistration.RegisterAllAreas();
        //    GlobalConfiguration.Configure(WebApiConfig.Register);
        //    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        //    RouteConfig.RegisterRoutes(RouteTable.Routes);
        //    BundleConfig.RegisterBundles(BundleTable.Bundles);
        //}
        public static void RegisterWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "NamedActionApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            var tele = new TelemetryClient();
            tele.TrackException(ex);


        }
    }
}

