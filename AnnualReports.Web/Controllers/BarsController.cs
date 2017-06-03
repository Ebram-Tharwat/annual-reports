using AnnualReports.Application.Core;
using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Common.Extensions;
using AnnualReports.Common.Utils;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.Extensions;
using AnnualReports.Web.ViewModels.BarModels;
using AnnualReports.Web.ViewModels.CommonModels;
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
                entities = _barService.GetAllBars(!string.IsNullOrEmpty(filter.DateAsYear) ? int.Parse(filter.DateAsYear) : -1, pagingInfo);
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
        public FileResult ExportBarsTemplate(int year)
        {
            //DateTime dateForBars = viewmodel.Date.HasValue ? viewmodel.Date.Value : DateTime.Now;
            MemoryStream stream = _exportingService.GetBarsTemplate(year);

            return File(stream, Constants.ExcelFilesMimeType,
                string.Format(Constants.BarsTemplateExcelFileName, year));
        }

        public ActionResult Upload()
        {
            var viewmodel = new BarsUploadViewModel();
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(BarsUploadViewModel viewmodel)
        {
            if (ModelState.IsValid) // validate file exist
            {
                if (viewmodel.ExcelFile != null && viewmodel.ExcelFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(viewmodel.ExcelFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Uploads/Bars/"), DateTime.Now.GetTimeStamp() + "_" + fileName);
                    List<Bar> addedEntities = new List<Bar>();
                    viewmodel.ExcelFile.SaveAs(path); // save a copy of the uploaded file.
                    // convert the uploaded file into datatable, then add/update db entities.
                    var dtBarsHours = ImportUtils.ImportXlsxToDataTable(viewmodel.ExcelFile.InputStream, true);
                    int numOfEntitiesUpdated = 0;
                    // load existed entities from DB, aka "cache".
                    var existedEntities = GetExistedBars(dtBarsHours).ToList();
                    foreach (var row in dtBarsHours.AsEnumerable().ToList())
                    {
                        var entityViewModel = new BarDetailsViewModel()
                        {
                            Year = int.Parse(row["Year"].ToString()),
                            BarNumber = row["State BARS Number"].ToString(),
                            MapToBarNumber = row["Map to"].ToString(),
                            DisplayName = row["Display Name"].ToString(),
                            Period = string.IsNullOrWhiteSpace(row["Period"].ToString()) ? (int?)null : int.Parse(row["Period"].ToString()),
                            IsActive = string.IsNullOrWhiteSpace(row["Is Active"].ToString()) ? false : row["Is Active"].ToString() == "1" ? true : false
                        };
                        var existedEntity = existedEntities.FirstOrDefault(t => t.BarNumber == entityViewModel.BarNumber && t.Year == entityViewModel.Year);
                        if (existedEntity == null)
                        {
                            var entity = Mapper.Map<BarDetailsViewModel, Bar>(entityViewModel);
                            addedEntities.Add(entity);
                        }
                        else
                        {
                            entityViewModel.Id = existedEntity.Id;
                            Mapper.Map(entityViewModel, existedEntity);
                            _barService.Update(existedEntity);
                            numOfEntitiesUpdated++;
                        }
                    }
                    if (addedEntities.Any())
                    {
                        _barService.Add(addedEntities);
                    }

                    Success($"<strong>{addedEntities.Count}</strong> records have been successfully added. <br\\>"
                        + $"<strong>{numOfEntitiesUpdated}</strong> records have been successfully updated.");
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Copy()
        {
            return View(new BarsCopyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Copy(BarsCopyViewModel viewmodel)
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
                    var copiedFunds = _barService.CopyBars(viewmodel.FromYear.Value, viewmodel.ToYear.Value);
                    Success($"<strong>{copiedFunds.Count}</strong> Bars have been successfully saved.");
                    return RedirectToAction("Index", new { year = viewmodel.ToYear.Value });
                }
                return View(viewmodel);
            }
            catch (Exception ex)
            {
                Danger("An error happened while updating Bars. Please try again.");
                return View(viewmodel);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var entity = _barService.GetById(id.Value);
            if (entity == null)
            {
                return HttpNotFound();
            }
            var viewmodel = Mapper.Map<Bar, BarEditViewModel>(entity);
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BarEditViewModel viewmodel)
        {
            Bar entity = null;
            if (ModelState.IsValid)
            {
                entity = _barService.GetById(viewmodel.Id);
                if (entity == null)
                {
                    return HttpNotFound();
                }
                Mapper.Map(viewmodel, entity);

                _barService.Update(entity);
                Success($"<strong>{entity.DisplayName} - {entity.BarNumber}</strong> was successfully updated.");
                return RedirectToAction("Index");
            }
            return View(viewmodel);
        }

        #region Helpers

        private IEnumerable<Bar> GetExistedBars(DataTable dtBars)
        {
            var groupByYear = dtBars.AsEnumerable().GroupBy(t => t["Year"]);
            foreach (var item in groupByYear)
            {
                var entities = _barService.GetByYear(int.Parse(item.Key.ToString()));
                foreach (var entity in entities)
                {
                    entity.BarNumber = entity.BarNumber.Trim();
                    yield return entity;
                }
            }
        }

        #endregion Helpers
    }
}