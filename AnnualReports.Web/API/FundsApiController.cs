using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.ViewModels.CommonModels;
using System.Web.Http;

namespace AnnualReports.Web.API
{
    [RoutePrefix("api/funds")]
    public class FundsApiController : ApiController
    {
        private readonly IFundService _fundService;

        public FundsApiController(IFundService fundService)
        {
            _fundService = fundService;
        }

        [Route("sync")]
        [HttpPost]
        public IHttpActionResult Sync(YearFilterViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                _fundService.SyncFunds(viewmodel.Year.Value, DbSource.ALL);
            }
            return Ok();
        }

        [Route("primary/{year}")]
        [HttpGet]
        public IHttpActionResult GetPrimaryFunds(int year)
        {
            return Ok(_fundService.GetPrimaryFunds(year, DbSource.ALL));
        }
    }
}