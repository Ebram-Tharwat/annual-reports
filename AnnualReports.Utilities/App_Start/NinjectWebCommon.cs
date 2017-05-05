using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Application.Core.Services;
using AnnualReports.Infrastructure.Core;
using AnnualReports.Infrastructure.Core.DbContexts.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.DbContexts.DistDb;
using AnnualReports.Infrastructure.Core.DbContexts.GcDb;
using AnnualReports.Infrastructure.Core.Interfaces;
using AnnualReports.Infrastructure.Core.Repositories.AnnualReportsDb;
using AnnualReports.Infrastructure.Core.Repositories.DistDb;
using AnnualReports.Infrastructure.Core.Repositories.GcDb;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(AnnualReports.Utilities.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(AnnualReports.Utilities.App_Start.NinjectWebCommon), "Stop")]

namespace AnnualReports.Utilities.App_Start
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using System;
    using System.Web;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            #region Db Contexts

            kernel.Bind<AnnualReportsDbContext>().ToSelf().InRequestScope();
            kernel.Bind<DistDbContext>().ToSelf().InRequestScope();
            kernel.Bind<GcDbContext>().ToSelf().InRequestScope();

            #endregion Db Contexts

            #region UOWs

            kernel.Bind<IUnitOfWork<AnnualReportsDbContext>>().To<UnitOfWork<AnnualReportsDbContext>>();
            kernel.Bind<IUnitOfWork<DistDbContext>>().To<UnitOfWork<DistDbContext>>();
            kernel.Bind<IUnitOfWork<GcDbContext>>().To<UnitOfWork<GcDbContext>>();

            #endregion UOWs

            #region Repositories

            kernel.Bind<IDistDbFundRepository>().To<DistDbFundRepository>();
            kernel.Bind<IGcDbFundRepository>().To<GcDbFundRepository>();

            kernel.Bind<IRepository<Domain.Core.AnnualReportsDbModels.Fund>>().To<AnnualReportsDbEfRepository<Domain.Core.AnnualReportsDbModels.Fund>>();

            #endregion Repositories

            #region Services

            kernel.Bind<IGPDynamicsService>().To<GPDynamicsService>();
            kernel.Bind<IFundService>().To<FundService>();

            #endregion Services
        }
    }
}