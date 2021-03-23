using Ninject;
using Ninject.Syntax;
using CardMGTService.Core.Services.Impl;
using CardMGTService.Core.Services.Interfaces;
using Ninject.Extensions.Conventions;
using System;
using System.Reflection;
using CardMGTService.Core.Repositories;
using CardMGTService.Core.Persistence.EF;
using CardMGTService.Core.Util;
using CardMGTService.Core.Persistence.Cache;
using CardMGTService.Core.Domain.Model;
using Microsoft.AspNet.Identity;
using CardMGTService.Core.Services.Security;

namespace CardMGTService.API.App_Start
{
    public static class NinjectIoc
    {
        public static Lazy<IKernel> CreateKernel = new Lazy<IKernel>(() =>
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            //kernel.Bind<ICacheManager>().To<MemoryCacheManager>().InSingletonScope();
            //kernel.Bind<GigMobilityDataContext>().ToSelf().InRequestScope();
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<ICardMGTService>().To<CardManagementService>();
            kernel.Bind<IApiClientService>().To<ApiClientService>();
            kernel.Bind<IMailer>().To<Mailer>();
            kernel.Bind<ILogService>().To<LogService>();
            kernel.Bind<ICacheManager>().To<CacheManager>();
            kernel.Bind<IServiceHelper>().To<ServiceHelper>();
            kernel.Bind<IUserManagerService<User>>().To<UserManagerService>();
            kernel.Bind<IPasswordHasher>().To<CardPasswordHasher>();
            kernel.Bind<IPasswordGenerator>().To<PasswordGenerator>();
         //RegisterRepositoriesByConvention(kernel);
            //RegisterServicesByConvention(kernel);

            return kernel;
        });

        public static TDependency Get<TDependency>()
        {
            return CreateKernel.Value.Get<TDependency>();
        }

        private static void RegisterRepositoriesByConvention(IBindingRoot root)
        {
            root.Bind(convention => convention
                .FromAssembliesMatching("*")
                .SelectAllClasses()
                .InheritedFrom(typeof(IRepository<>))
                .BindDefaultInterfaces()
            );
        }

        //private static void RegisterServicesByConvention(IBindingRoot root)
        //{
        //    root.Bind(convention => convention
        //        .FromAssembliesMatching("*")
        //        .SelectAllClasses()
        //        .InheritedFrom(typeof(IServiceDependencyMarker))
        //        .BindDefaultInterfaces()
        //    );
        //}
    }

}