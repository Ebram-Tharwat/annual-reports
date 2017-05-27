using AnnualReports.Application.Core;
using AnnualReports.Application.Core.Interfaces;
using System.IO;
using System.Web.Mvc;

namespace AnnualReports.Web.Controllers
{
    [RoutePrefix("reports")]
    public class ReportsController : Controller
    {
        private readonly IFundService _fundService;
        private readonly IExportingService _exportingService;

        public ReportsController(IFundService fundService, IExportingService exportingService)
        {
            _fundService = fundService;
            _exportingService = exportingService;
        }

        public ActionResult FundsAnnualReport()
        {
            return View();
        }

        [HttpGet]
        [Route("ExportFundsAnnualReportToExcel/{year:int}")]
        public ActionResult ExportFundsAnnualReportToExcel(int year)
        {
            MemoryStream stream = _exportingService.GetFundsAnnualReportExcel(year);

            return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.FundsAnnualReportExcelFileName, year));
        }
    }
}