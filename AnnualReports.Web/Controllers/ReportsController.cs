using AnnualReports.Application.Core;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Application.Core.UseCases;
using AnnualReports.Common.Extensions;
using AnnualReports.Common.Utils;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.ViewModels.ReportModels;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AnnualReports.Web.Controllers
{
    [RoutePrefix("reports")]
    [Authorize]
    public class ReportsController : BaseController
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
            viewmodel.MonthlyReportRules = _journalVoucherReportUseCase.GetMonthlyReportRules();
            viewmodel.MonthlyImportExceptionRule = _journalVoucherReportUseCase.GetMonthlyImportExceptionRules();
            return View(viewmodel);
        }

        [HttpGet]
        [Route("journal-voucher-editImportRule")]
        public ActionResult EditImportRule(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                return View(_journalVoucherReportUseCase.GetMonthlyImportExceptionRuleReport(int.Parse(id)));
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [Route("journal-voucher-editImportRule")]
        [ValidateAntiForgeryToken]
        public ActionResult EditImportRule(MonthlyImportFundExceptionRule model)
        {
            if (ModelState.IsValid)
            {
                var result = _journalVoucherReportUseCase.GetMonthlyImportExceptionRuleReport(model.Id);
                if (result == null)
                {
                    return HttpNotFound();
                }
                result.NewFundId = model.NewFundId;
                result.OriginalFundId = model.OriginalFundId;
                result = _journalVoucherReportUseCase.UpdateMonthlyImportExceptionRuleReport(result);
                if (result == null)
                {
                    return HttpNotFound();
                }
                return RedirectToAction("JournalVoucherReport");
            }
            ModelState.AddModelError("", "An error occurred.");
            return View();
        }

        [HttpGet]
        [Route("journal-voucher-createImportRule")]
        public ActionResult CreateImportRule()
        {
            var model = new MonthlyImportFundExceptionRule();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("journal-voucher-createImportRule")]
        public ActionResult Create(MonthlyImportFundExceptionRule model)
        {
            if (ModelState.IsValid)
            {
                _journalVoucherReportUseCase.AddMonthlyImportFundExceptionRuleReport(model);

                Success($"<strong>{model.OriginalFundId} - {model.NewFundId}</strong> was successfully saved.");
                return RedirectToAction("JournalVoucherReport");
            }
            return View(model);
        }

        [HttpGet]
        [Route("journal-voucher-edit")]
        public ActionResult EditRule(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                return View(_journalVoucherReportUseCase.GetMonthlyReport(int.Parse(id)));
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        [Route("journal-voucher-edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditRule(MonthlyReportRule model)
        {
            if (ModelState.IsValid)
            {
                var result = _journalVoucherReportUseCase.GetMonthlyReport(model.Id);
                if (result == null)
                {
                    return HttpNotFound();
                }
                result.CreditAccount = model.CreditAccount;
                result.DebitAccount = model.DebitAccount;
                result.CreditExceptionNegative = string.IsNullOrWhiteSpace(model.CreditExceptionNegative) ? null : model.CreditExceptionNegative;
                result.DebitExceptionNegative = string.IsNullOrWhiteSpace(model.DebitExceptionNegative) ? null : model.DebitExceptionNegative;
                result.FundIds = model.FundIds;
                result = _journalVoucherReportUseCase.UpdateMonthlyReport(result);
                if (result == null)
                {
                    return HttpNotFound();
                }
                return RedirectToAction("JournalVoucherReport");
            }
            ModelState.AddModelError("", "An error occurred.");
            return View();
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
                    viewmodel.MonthlyImportExceptionRule = _journalVoucherReportUseCase.GetMonthlyImportExceptionRules();
                    MemoryStream stream = _journalVoucherReportUseCase.Execute(viewmodel.ExcelFile.InputStream,
                                                                               viewmodel.Date.Value.Year,
                                                                               viewmodel.MonthlyImportExceptionRule);

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