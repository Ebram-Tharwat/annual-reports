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
    using AnnualReports.Application.Core.ExcelProcessors.AuditorMaster;
    using AnnualReports.Application.Core.UseCases;
    using AnnualReports.Domain.Core.AnnualReportsDbModels;
    using AnnualReports.Infrastructure.Core.Repositories;
    using Domain.Core.DistDbModels;
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

        public static object CurrentDbContext
        {
            get
            {
                if (currentKernal == null)
                {
                    return null;
                }
                return currentKernal.Get(typeof(AnnualReportsDbContext));
            }
        }

        private static StandardKernel currentKernal = null;

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
                currentKernal = kernel;
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

            kernel.Bind<DistDbEfRepository<Gl00100>>().ToSelf().InRequestScope();
            kernel.Bind<GcDbEfRepository<AnnualReports.Domain.Core.GcDbModels.Gl00100>>().ToSelf().InRequestScope();
            kernel.Bind<IDistDbFundRepository>().To<DistDbFundRepository>();
            kernel.Bind<IGcDbFundRepository>().To<GcDbFundRepository>();

            kernel.Bind<IAnnualReportsDbFundRepository>().To<AnnualReportsDbFundRepository>();
            kernel.Bind<IAnnualReportsDbBarRepository>().To<AnnualReportsDbBarRepository>();
            kernel.Bind<IMappingRuleRepository>().To<MappingRuleRepository>();
            kernel.Bind<IMappingRuleService>().To<MappingRuleService>();

            #endregion Repositories

            #region Services

            kernel.Bind<IGPDynamicsService>().To<GPDynamicsService>();
            kernel.Bind<IFundService>().To<FundService>();
            kernel.Bind<IExportingService>().To<ExcelExportingService>();
            kernel.Bind<IBarService>().To<BarService>();
            kernel.Bind<IReportService>().To<ReportService>();
            kernel.Bind<IMonthlyReportRepository>().To<MonthlyReportRepository>();
            kernel.Bind<IGenerateJournalVoucherReportUseCase>().To<GenerateJournalVoucherReportUseCase>();
            kernel.Bind<AuditorMasterProcessor>().To<WarrantsSheetProcessor>();
            kernel.Bind<AuditorMasterProcessor>().To<TaxesSheetProcessor>();
            kernel.Bind<AuditorMasterProcessor>().To<InvestmentsSheetProcessor>();
            kernel.Bind<AuditorMasterProcessor>().To<WarrantsInterestSheetProcessor>();

            #endregion Services
        }
    }
}