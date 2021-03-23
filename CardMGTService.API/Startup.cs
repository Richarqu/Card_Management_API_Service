using System;
using System.Collections.Generic;
using System.Linq;
using CardMGTService.API.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System.Web.Http;
using Ninject.Web.Common.OwinHost;
using CardMGTService.API.App_Start;
using Ninject.Web.WebApi.OwinHost;
using CardMGTService.Core.Services.Interfaces;
using CardMGTService.Core.Domain.Model;
using Microsoft.Owin.Security.Facebook;

[assembly: OwinStartup(typeof(CardMGTService.API.Startup))]

namespace CardMGTService.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            app.UseNinjectMiddleware(() => NinjectIoc.CreateKernel.Value)
               .UseNinjectWebApi(config);
        }
        public void ConfigureOAuth(IAppBuilder app)
        {
            Func<IUserManagerService<User>> userMgr = () => NinjectIoc.Get<IUserManagerService<User>>();
          

            var oAuthServerOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                AllowInsecureHttp = true,
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(8),
                Provider = new ApplicationOAuthProvider(userMgr)
            };

            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
           
        }
    }
}
