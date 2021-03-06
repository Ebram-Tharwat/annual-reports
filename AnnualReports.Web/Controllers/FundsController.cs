﻿using AnnualReports.Application.Core;
using AnnualReports.Application.Core.Contracts.FundEntities;
using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Common.Extensions;
using AnnualReports.Common.Utils;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.Extensions;
using AnnualReports.Web.ViewModels.CommonModels;
using AnnualReports.Web.ViewModels.FundModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AnnualReports.Web.Controllers
{
    [Authorize]
    public class FundsController : BaseController
    {
        private readonly IFundService _fundService;
        private readonly IExportingService _exportingService;
        private readonly IGPDynamicsService _gpDynamicsService;
        private const string FilterDateKey = "FundsFilter";

        public FundsController(IFundService fundService, IExportingService exportingService, IGPDynamicsService gpDynamicsService)
        {
            _fundService = fundService;
            _exportingService = exportingService;
            _gpDynamicsService = gpDynamicsService;
        }

        public ActionResult Index(YearFilterViewModel filters, int page = 1)
        {
            var pagingInfo = new PagingInfo() { PageNumber = page };
            var entities = Enumerable.Empty<Fund>();
            // keep track of filter across
            if (filters != null && !filters.IsEmpty)
            {
                TempData[FilterDateKey] = filters;
            }
            else
            {
                if (TempData.Peek(FilterDateKey) != null)
                    filters = TempData.Peek(FilterDateKey) as YearFilterViewModel;
            }

            if (filters.Year.HasValue)
                entities = _fundService.GetAllFunds(filters.Year.Value, filters.DbSource, filters.DisplayName, filters.FundNumber, null, pagingInfo);
            var viewmodel = entities.ToMappedPagedList<Fund, FundDetailsViewModel>(pagingInfo);

            ViewBag.FilterViewModel = filters;
            ViewBag.AvailableDbSources = new List<SelectListItem>() {
                 new SelectListItem() {Text = DbSource.GC.ToString(), Value = ((int)DbSource.GC).ToString() },
                 new SelectListItem() {Text = DbSource.DIST.ToString(), Value = ((int)DbSource.DIST).ToString() }
                 };
            return View(viewmodel);
        }

        [HttpGet]
        public FileResult ExportFundsTemplate(int year)
        {
            // 1- sync data from GP Dynamics.
            _fundService.SyncFunds(year, DbSource.ALL);
            // 2- generate template.
            MemoryStream stream = _exportingService.GetFundsTemplate(year);
            return File(stream, Constants.ExcelFilesMimeType, string.Format(Constants.FundsTemplateExcelFileName, year));
        }

        public ActionResult Create()
        {
            var viewmodel = new FundAddViewModel();
            viewmodel.AvailableDbSources = new List<SelectListItem>() {
                 new SelectListItem() {Text = DbSource.GC.ToString(), Value = ((int)DbSource.GC).ToString() },
                 new SelectListItem() {Text = DbSource.DIST.ToString(), Value = ((int)DbSource.DIST).ToString() }
                 };
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FundAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                if (_gpDynamicsService.GetAllFunds().FirstOrDefault(t => t.Number.Trim() == viewmodel.FundNumber) == null)
                {
                    Danger($"This fund number <strong>{viewmodel.FundNumber}</strong>, does not exist in GP Databse");
                }
                else if (_fundService.GetByFundNumberAndYear(viewmodel.FundNumber, viewmodel.Year.Value) != null)
                {
                    Danger($"A Fund with same Number <strong>{viewmodel.FundNumber}</strong> already exists within the same year <strong>{viewmodel.Year.Value}</strong>.");
                }
                else
                {
                    var entity = Mapper.Map<FundAddViewModel, Fund>(viewmodel);
                    _fundService.Add(new List<Fund>() { entity });

                    Success($"<strong>{entity.DisplayName} - {entity.FundNumber}</strong> was successfully saved.");
                    return RedirectToAction("Index", new { year = entity.Year, dbsource = entity.DbSource });
                }
            }
            viewmodel.AvailableDbSources = new List<SelectListItem>() {
                 new SelectListItem() {Text = DbSource.GC.ToString(), Value = ((int)DbSource.GC).ToString() },
                 new SelectListItem() {Text = DbSource.DIST.ToString(), Value = ((int)DbSource.DIST).ToString() }
                 };
            return View(viewmodel);
        }

        public ActionResult Upload()
        {
            var viewmodel = new FundsUploadViewModel();
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(FundsUploadViewModel viewmodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (viewmodel.ExcelFile != null && viewmodel.ExcelFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(viewmodel.ExcelFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Uploads/Funds/"),
                            DateTime.Now.GetTimeStamp() + "_" + fileName);
                        viewmodel.ExcelFile.SaveAs(path); // save a copy of the uploaded file.
                        // convert the uploaded file into datatable, then add/update db entities.
                        var dtDistFunds = ImportUtils.ImportXlsxToDataTable(viewmodel.ExcelFile.InputStream, true, 1, true);
                        var dtGcFunds = ImportUtils.ImportXlsxToDataTable(viewmodel.ExcelFile.InputStream, true, 2, true);

                        var distFundAddEntities = dtDistFunds.AsEnumerable().Select(row => new FundAddEntity()
                        {
                            Year = int.Parse(row["Year"].ToString()),
                            FundNumber = row["Fund Number"].ToString(),
                            DisplayName = row["Display Name"].ToString(),
                            MCAG = row["MCAG"].ToString(),
                            MapTo = row["Map to"].ToString(),
                            IsActive = int.Parse(row["Is Active"].ToString()) == 1,
                            DbSource = DbSource.DIST
                        });

                        var gcFundAddEntities = dtGcFunds.AsEnumerable().Select(row => new FundAddEntity()
                        {
                            Year = int.Parse(row["Year"].ToString()),
                            FundNumber = row["Fund Number"].ToString(),
                            DisplayName = row["Display Name"].ToString(),
                            MCAG = row["MCAG"].ToString(),
                            MapTo = row["Map to"].ToString(),
                            IsActive = int.Parse(row["Is Active"].ToString()) == 1,
                            DbSource = DbSource.GC
                        });

                        List<FundAddEntity> rejectedFunds;
                        var uploadedFunds = distFundAddEntities.Union(gcFundAddEntities).ToList();
                        var validFunds = _fundService.AddUploadedFunds(viewmodel.FundsYear.Value, uploadedFunds, out rejectedFunds);
                        Success($"<strong>{validFunds.Count}</strong> Funds have been successfully saved.");
                        if (rejectedFunds.Any())
                            Danger($"<strong>{rejectedFunds.Count}</strong> Funds have been skipped. Please make sure that they exist in GP Dynamics system.");
                    }
                }
                return RedirectToAction("Index", new { year = viewmodel.FundsYear.Value });
            }
            catch
            {
                Danger("An error happened while updating Funds. Please try again.");
                return View(viewmodel);
            }
        }

        public ActionResult Copy()
        {
            return View(new FundsCopyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy(FundsCopyViewModel viewmodel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (viewmodel.FromYear.Value == viewmodel.ToYear.Value)
                    {
                        ModelState.AddModelError("", "The to year field and from year field cannot be the same");
                        return View(viewmodel);
                    }
                    var copiedFunds = _fundService.CopyFunds(viewmodel.FromYear.Value, viewmodel.ToYear.Value);
                    Success($"<strong>{copiedFunds.Count}</strong> Funds have been successfully saved.");
                    return RedirectToAction("Index", new { year = viewmodel.ToYear.Value });
                }
                return View(viewmodel);
            }
            catch
            {
                Danger("An error happened while updating Funds. Please try again.");
                return View(viewmodel);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Fund entity = _fundService.GetById(id.Value);
            if (entity == null)
            {
                return HttpNotFound();
            }
            var viewmodel = Mapper.Map<Fund, FundEditViewModel>(entity);
            viewmodel.PrimaryFunds = _fundService.GetPrimaryFunds(entity.Year, entity.DbSource)
                .Select(t => new SelectListItem()
                {
                    Text = t.DisplayName + " - " + t.FundNumber,
                    Value = t.Id.ToString()
                });
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FundEditViewModel viewmodel)
        {
            Fund entity = null;
            if (ModelState.IsValid)
            {
                entity = _fundService.GetById(viewmodel.Id);
                if (entity == null)
                {
                    return HttpNotFound();
                }
                Mapper.Map(viewmodel, entity);

                _fundService.Update(entity);
                Success($"<strong>{entity.DisplayName} - {entity.FundNumber}</strong> was successfully updated.");
                return RedirectToAction("Index");
            }
            entity = _fundService.GetById(viewmodel.Id);
            if (entity == null)
            {
                return HttpNotFound();
            }
            viewmodel.PrimaryFunds = _fundService.GetPrimaryFunds(entity.Year, entity.DbSource)
                .Select(t => new SelectListItem()
                {
                    Text = t.DisplayName + " - " + t.FundNumber,
                    Value = t.Id.ToString()
                });
            return View(viewmodel);
        }
    }
}