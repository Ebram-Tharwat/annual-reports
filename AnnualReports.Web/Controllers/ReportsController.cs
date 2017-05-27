using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AnnualReports.Application.Core.Services;

namespace AnnualReports.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly FundService _fundService;

        public ReportsController(FundService fundService)
        {
            _fundService = fundService;
        }

        public ActionResult FundsAnnualReport()
        {
            return View();
        }
    }
}