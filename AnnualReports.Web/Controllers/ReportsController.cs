using AnnualReports.Application.Core;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Common.Extensions;
using AnnualReports.Common.Utils;
using AnnualReports.Web.ViewModels.ReportModels;
using System;
using System.Data;
using System.IO;
using System.Linq;
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

        [HttpGet]
        public ActionResult AnnualReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult MonthlyReport()
        {
            var viewmodel = new MonthlyReportGenerateViewModel();
            ViewBag.DisplayResults = false;
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyReport(MonthlyReportGenerateViewModel viewmodel)
        {
            if (ModelState.IsValid) // validate file exist
            {
                if (viewmodel.ExcelFile != null && viewmodel.ExcelFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(viewmodel.ExcelFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/MonthlyReport/"), DateTime.Now.GetTimeStamp() + "_" + fileName);
                    viewmodel.ExcelFile.SaveAs(path); // save a copy of the uploaded file.
                    // convert the uploaded file into datatable, then add/update db entities.
                    var dtBarsHours = ImportUtils.ImportXlsxToDataTable(viewmodel.ExcelFile.InputStream, true, 1);
                    var excelData = dtBarsHours.AsEnumerable().Select(row =>
                    {
                        string account = row["Account"].ToString();
                        if (account.StartsWith("-"))
                            account = account.Remove(0, 1);

                        return new MonthlyReportDataViewModel()
                        {
                            AccountNumber = account,
                            Date = DateTime.Parse(row["Date"].ToString()),
                            Amount = decimal.Parse(row["Amount"].ToString()),
                            Description = row["Description"].ToString(),
                            Category = row["Category"].ToString(),
                        };
                    }).ToList();

                    viewmodel.Data = excelData;
                    ViewBag.DisplayResults = true;
                }
            }
            else
            {
                ViewBag.DisplayResults = false;
            }
            return View(viewmodel);
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