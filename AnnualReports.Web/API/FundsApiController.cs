using AnnualReports.Application.Core.Interfaces;
using AnnualReports.Domain.Core.AnnualReportsDbModels;
using System.Web.Http;
using AnnualReports.Web.ViewModels.CommonModels;

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
    }
}