using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.Extensions;
using AnnualReports.Web.ViewModels.CommonModels;
using AnnualReports.Web.ViewModels.MappingRuleModels;
using AutoMapper;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AnnualReports.Web.Controllers
{
    [Authorize]
    public class MappingRulesController : BaseController
    {
        private readonly IMappingRuleService _mappingRuleService;

        public MappingRulesController(IMappingRuleService mappingRuleService)
        {
            this._mappingRuleService = mappingRuleService;
        }

        // GET: MappingRules
        public ActionResult Index(YearFilterViewModel filters, int page = 1)
        {
            var pagingInfo = new PagingInfo() { PageNumber = page };
            var entities = Enumerable.Empty<MappingRule>();
            if (TryValidateModel(filters))
            {
                entities = _mappingRuleService.GetAll(filters.Year, filters.FundNumber, pagingInfo);
                ViewBag.DisplayResults = true;
            }
            else
            {
                ViewBag.DisplayResults = false;
            }
            var viewmodel = entities.ToMappedPagedList<MappingRule, MappingRuleDetailsViewModel>(pagingInfo);

            ViewBag.FilterViewModel = filters;
            return View(viewmodel);
        }

        public ActionResult Create()
        {
            var viewmodel = new MappingRuleAddViewModel();
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MappingRuleAddViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                var entity = Mapper.Map<MappingRuleAddViewModel, MappingRule>(viewmodel);
                _mappingRuleService.Add(entity);

                Success($"<strong>Mapping Rule</strong> was successfully saved.");
                return RedirectToAction("Index", new { year = entity.Year });
            }
            return View(viewmodel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var entity = _mappingRuleService.GetById(id.Value);
            if (entity == null)
            {
                return HttpNotFound();
            }
            var viewmodel = Mapper.Map<MappingRule, MappingRuleEditViewModel>(entity);
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MappingRuleEditViewModel viewmodel)
        {
            MappingRule entity = null;
            if (ModelState.IsValid)
            {
                entity = _mappingRuleService.GetById(viewmodel.Id);
                if (entity == null)
                {
                    return HttpNotFound();
                }
                Mapper.Map(viewmodel, entity);

                _mappingRuleService.Update(entity);
                Success($"<strong>Mapping Rule</strong> was successfully updated.");
                return RedirectToAction("Index", new { year = entity.Year });
            }
            return View(viewmodel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entity = _mappingRuleService.GetById(id.Value);
            if (entity == null)
            {
                return HttpNotFound();
            }
            var viewmodel = Mapper.Map<MappingRule, MappingRuleDetailsViewModel>(entity);
            return View(viewmodel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var entity = _mappingRuleService.GetById(id);
            if (entity != null) _mappingRuleService.Remove(entity);
            Success($"<strong>Mapping Rule</strong> was successfully deleted.");
            return RedirectToAction("Index", new { year = entity.Year });
        }
    }
}