using AnnualReports.Application.Core;
using AnnualReports.Application.Core.Interfaces;
using System.IO;
using System.Web.Mvc;

namespace AnnualReports.Web.Controllers
{
    [RoutePrefix("reports")]
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IFundService _fundService;
        private readonly IExportingService _exportingService;

        public ReportsController(IFundService fundService, IExportingService exportingService)
        {
            _fundService = fundService;
            _exportingService = exportingService;
        }

        public ActionResult AnnualReport()
        {
            return View();
        }

        [HttpGet]
        [Route("ExportAnnualReportToExcel/{year:int}/{fundId:int?}")]
        public ActionResult ExportAnnualReportToExcel(int year, int? fundId)
        {
            MemoryStream stream = _exportingService.GetAnnualReportExcel(year, fundId);

            return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.AnnualReportExcelFileName, year));
        }
    }
}