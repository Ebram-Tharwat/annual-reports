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
using AnnualReports.Common.Extensions;
using AnnualReports.Common.Utils;
using System.Data;
using AutoMapper;

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
        public FileResult ExportBarsTemplate(int year)
        {
            //DateTime dateForBars = viewmodel.Date.HasValue ? viewmodel.Date.Value : DateTime.Now;
            MemoryStream stream = _exportingService.GetBarsTemplate(year);

            return File(stream, Constants.ExcelFilesMimeType,
                string.Format(Constants.BarsTemplateExcelFileName , year));
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
                    List<Tuple<int, string>> barNumberWithMapToBarIdList = new List<Tuple<int, string>>();
                    foreach (var row in dtBarsHours.AsEnumerable().ToList())
                    {
                        var entityViewModel = new BarDetailsViewModel()
                        {
                            Year = int.Parse(row["Year"].ToString()),
                            BarNumber = int.Parse(row["State BARS Number"].ToString()),
                            MapToBarId = int.Parse(row["Map to"].ToString()),
                            DisplayName = row["Display Name"].ToString(),
                            IsActive = string.IsNullOrWhiteSpace(row["Is Active"].ToString()) ? false : row["Is Active"].ToString() == "1" ? true : false
                        };
                        //var existedBar = _barService.GetByBarNumber(entityViewModel.BarNumber);
                        //if (existedBar == null)
                        //{
                        //    ModelState.AddModelError("", $"Invalid Bar Id with value ={entityViewModel.BarNumber}");
                        //}
                        // check if entity already exists.
                        var existedEntity = existedEntities.FirstOrDefault(t => t.BarNumber == entityViewModel.BarNumber.ToString()
                         && t.Year == entityViewModel.Year);
                        if (existedEntity == null)
                        {
                            var entity = Mapper.Map<BarDetailsViewModel, Bar>(entityViewModel);
                            barNumberWithMapToBarIdList.Add(new Tuple<int, string>(entityViewModel.BarNumber, entityViewModel.MapToBarId.ToString()));
                            entity.MapToBarId = null;
                            //entity.Id = existedEntity.Id;
                            addedEntities.Add(entity);
                        }
                        else
                        {
                            entityViewModel.Id = existedEntity.Id;
                            var mapToBar = _barService.GetByBarNumber(entityViewModel.MapToBarId);
                            if(mapToBar != null)
                            {
                                entityViewModel.MapToBarId = mapToBar.Id;
                            }
                            else
                            {
                                entityViewModel.MapToBarId = existedEntity.MapToBarId.Value;
                            }
                            Mapper.Map(entityViewModel, existedEntity);
                            _barService.Update(existedEntity);
                            numOfEntitiesUpdated++;
                        }
                    }
                    if (addedEntities.Any())
                    {
                        _barService.Add(addedEntities);
                        foreach (var item in addedEntities)
                        {
                            var addedItem = _barService.GetByBarNumber(int.Parse(barNumberWithMapToBarIdList.Where(t => t.Item1 == int.Parse(item.BarNumber)).FirstOrDefault().Item2));
                            item.MapToBarId = addedItem.Id;
                            _barService.Update(item);
                        }
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
            catch(Exception ex)
            {
                Danger("An error happened while updating Bars. Please try again.");
                return View(viewmodel);
            }
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
        #endregion
    }
}