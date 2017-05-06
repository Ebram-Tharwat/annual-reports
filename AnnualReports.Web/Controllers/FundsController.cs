using AnnualReports.Application.Core.Contracts.Paging;
using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.Extensions;
using AnnualReports.Web.ViewModels.CommonModels;
using AnnualReports.Web.ViewModels.FundModels;
using System.Web.Mvc;

namespace AnnualReports.Web.Controllers
{
    public class FundsController : Controller
    {
        private readonly IFundService _fundService;

        public FundsController(IFundService fundService)
        {
            _fundService = fundService;
        }

        public ActionResult Index(YearFilterViewModel filters, int page = 1)
        {
            var pagingInfo = new PagingInfo() { PageNumber = page };
            var entities = _fundService.GetAllFunds((short)filters.Year.Value, DbSource.ALL, pagingInfo);
            var viewmodel = entities.ToMappedPagedList<Fund, FundDetailsViewModel>(pagingInfo);

            ViewBag.FilterViewModel = filters;
            return View(viewmodel);
        }
    }
}