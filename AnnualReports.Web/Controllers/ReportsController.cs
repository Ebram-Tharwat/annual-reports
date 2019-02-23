using AnnualReports.Application.Core;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Application.Core.UseCases;
using AnnualReports.Common.Extensions;
using AnnualReports.Common.Utils;
using AnnualReports.Web.ViewModels.ReportModels;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnnualReports.Web.Controllers
{
    [RoutePrefix("reports")]
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IFundService _fundService;
        private readonly IExportingService _exportingService;
        private readonly IGenerateJournalVoucherReportUseCase _journalVoucherReportUseCase;

        public ReportsController(IFundService fundService, IExportingService exportingService, IGenerateJournalVoucherReportUseCase journalVoucherReportUseCase)
        {
            _fundService = fundService;
            _exportingService = exportingService;
            _journalVoucherReportUseCase = journalVoucherReportUseCase;
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

        [HttpGet]
        public ActionResult DistExceptionReport()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GcExceptionReport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonthlyReport(MonthlyReportGenerateViewModel viewmodel)
        {
            if (ModelState.IsValid) // validate file exist
            {
                if (viewmodel.ExcelFile != null && viewmodel.ExcelFile.ContentLength > 0)
                {
                    SaveUploadedFile(viewmodel.ExcelFile, Server.MapPath("~/Uploads/MonthlyReport/"));

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
        [Route("journal-voucher")]
        public ActionResult JournalVoucherReport()
        {
            var viewmodel = new ReportFiltersViewModel();
            return View(viewmodel);
        }

        [HttpPost]
        [Route("journal-voucher")]
        [ValidateAntiForgeryToken]
        public ActionResult JournalVoucherReport(ReportFiltersViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                if (viewmodel.ExcelFile != null && viewmodel.ExcelFile.ContentLength > 0)
                {
                    SaveUploadedFile(viewmodel.ExcelFile, Server.MapPath("~/Uploads/WarrantReport/"));
                    MemoryStream stream = _journalVoucherReportUseCase.Execute(viewmodel.ExcelFile.InputStream, viewmodel.Date.Value.Year);

                    return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.JournalVoucherReportExcelFileName, viewmodel.Date.Value.Year));
                }
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

        [HttpGet]
        [Route("ExportDistExceptionReportToExcel/{year:int}")]
        public ActionResult ExportDistExceptionReportToExcel(int year)
        {
            MemoryStream stream = _exportingService.GetDistExceptionReportExcel(year);
            return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.DistExceptionReportExcelFileName, year));
        }

        [HttpGet]
        [Route("ExportGcExceptionReportToExcel/{year:int}")]
        public ActionResult ExportGcExceptionReportToExcel(int year)
        {
            MemoryStream stream = _exportingService.GetGcExceptionReportExcel(year);
            return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.GcExceptionReportExcelFileName, year));
        }

        private void SaveUploadedFile(HttpPostedFileBase file, string folderPath)
        {
            Directory.CreateDirectory(folderPath);
            var fileName = Path.GetFileName(file.FileName);
            var fullPath = Path.Combine(folderPath, DateTime.Now.GetTimeStamp() + "_" + fileName);
            file.SaveAs(fullPath);
        }
    }
}