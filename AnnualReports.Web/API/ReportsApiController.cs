using AnnualReports.Application.Core.Interfaces;
using System.Web.Http;

namespace AnnualReports.Web.API
{
    [RoutePrefix("api/report")]
    public class ReportsApiController : ApiController
    {
        private readonly IReportService _reportService;
        private readonly IFundService _fundService;

        public ReportsApiController(IReportService reportService, IFundService fundService)
        {
            _reportService = reportService;
            _fundService = fundService;
        }

        [Route("annualreport/{year:int}/{fundId?}")]
        [HttpGet]
        public IHttpActionResult GetReportData(int year, int? fundId = null)
        {
            return Ok(_reportService.GetAnnualReportData(year, fundId));
        }
    }
}