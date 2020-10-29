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
        [Route("annual-report")]
        public ActionResult AnnualReport()
        {
            return View();
        }

        [HttpGet]
        [Route("annual-report/year/{year:int}/export/excel")]
        public ActionResult ExportAnnualReportToExcel(int year, int? fundId = null)
        {
            MemoryStream stream = _exportingService.GetAnnualReportExcel(year, fundId);

            return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.AnnualReportExcelFileName, year));
        }

        [HttpGet]
        [Route("annual-report/year/{year:int}/fundId/{fundId:int}/export/excel")]
        public ActionResult ExportAnnualReportToExcelByFundId(int year, int fundId)
        {
            MemoryStream stream = _exportingService.GetAnnualReportExcel(year, fundId);

            return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.AnnualReportExcelFileName, year));
        }

        [HttpGet]
        [Route("monthly-report")]
        public ActionResult MonthlyReport()
        {
            var viewmodel = new MonthlyReportGenerateViewModel();
            ViewBag.DisplayResults = false;
            return View(viewmodel);
        }

        [HttpPost]
        [Route("monthly-report")]
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
        [Route("dist-exception-report")]
        public ActionResult DistExceptionReport()
        {
            return View();
        }

        [HttpGet]
        [Route("dist-exception-report/year/{year:int}/export/excel")]
        public ActionResult ExportDistExceptionReportToExcel(int year)
        {
            MemoryStream stream = _exportingService.GetDistExceptionReportExcel(year);
            return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.DistExceptionReportExcelFileName, year));
        }

        [HttpGet]
        [Route("gc-exception-report")]
        public ActionResult GcExceptionReport()
        {
            return View();
        }

        [HttpGet]
        [Route("gc-exception-report/year/{year:int}/export/excel")]
        public ActionResult ExportGcExceptionReportToExcel(int year)
        {
            MemoryStream stream = _exportingService.GetGcExceptionReportExcel(year);
            return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.GcExceptionReportExcelFileName, year));
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
        [Route("journal-voucher/rule/add")]
        public ActionResult AddJournalVoucherRule()
        {
            var model = new MonthlyReportRule();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("journal-voucher/rule/add")]
        public ActionResult AddJournalVoucherRule(MonthlyReportRule model)
        {
            if (ModelState.IsValid)
            {
                _journalVoucherReportUseCase.AddJournalVoucherRule(model);
                Success($"<strong>{model.Description}</strong> for <strong>{model.FundIds}</strong> was successfully saved.");
                return RedirectToAction("JournalVoucherReport");
            }
            return View(model);
        }

        [HttpGet]
        [Route("journal-voucher/rule/edit")]
        public ActionResult EditJournalVoucherRule(string id)
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
        [Route("journal-voucher/rule/edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditJournalVoucherRule(MonthlyReportRule model)
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

        [HttpGet]
        [Route("monthly-import-exception-rule/edit")]
        public ActionResult EditMonthlyImportExceptionRule(string id)
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
        [Route("monthly-import-exception-rule/edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditMonthlyImportExceptionRule(MonthlyImportFundExceptionRule model)
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
        [Route("monthly-import-exception-rule/add")]
        public ActionResult AddMonthlyImportExceptionRule()
        {
            var model = new MonthlyImportFundExceptionRule();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("monthly-import-exception-rule/add")]
        public ActionResult AddMonthlyImportExceptionRule(MonthlyImportFundExceptionRule model)
        {
            if (ModelState.IsValid)
            {
                _journalVoucherReportUseCase.AddMonthlyImportFundExceptionRuleReport(model);

                Success($"<strong>{model.OriginalFundId} - {model.NewFundId}</strong> was successfully saved.");
                return RedirectToAction("JournalVoucherReport");
            }
            return View(model);
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