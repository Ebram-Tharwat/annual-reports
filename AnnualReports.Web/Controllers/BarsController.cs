using AnnualReports.Application.Core;
using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.Extensions;
using AnnualReports.Web.ViewModels.BarModels;
using AnnualReports.Web.ViewModels.CommonModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnnualReports.Web.Controllers
{
    public class BarsController : BaseController
    {
        private IBarService _barService;
        
        private IExportingService _exportingService;

        public BarsController(IBarService barService, IExportingService exportingService)
        {
            _barService = barService;
            _exportingService = exportingService;
        }

        public ActionResult Index(YearFilterViewModel filter, int page = 1)
        {
            var pagingInfo = new PagingInfo() { PageNumber = page };
            var entities = Enumerable.Empty<Bar>();
            if (TryValidateModel(filter))
            {
                entities = _barService.GetAllBars(!string.IsNullOrEmpty(filter.DateAsYear)?int.Parse(filter.DateAsYear):-1, pagingInfo);
                ViewBag.DisplayResults = true;
            }
            else
            {
                ViewBag.DisplayResults = false;
            }

            var viewmodel = new BarsListViewModel()
            {
                Filters = filter,
                Data = entities.ToMappedPagedList<Bar, BarDetailsViewModel>(pagingInfo)
            };
            return View(viewmodel);
        }

        [HttpGet]
        public FileResult ExportBarsTemplate(BarsUploadViewModel viewmodel)
        {
            DateTime dateForBars = viewmodel.Date.HasValue ? viewmodel.Date.Value : DateTime.Now;
            MemoryStream stream = _exportingService.GetBarsTemplate(viewmodel.Date.HasValue?viewmodel.Date.Value.Year:DateTime.Now.Year);

            return File(stream, Constants.ExcelFilesMimeType,
                string.Format(Constants.BarsTemplateExcelFileName
                , CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateForBars.Month)
                , dateForBars.Year));
        }
    }
}