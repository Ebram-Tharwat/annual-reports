using System.Web.Mvc;

namespace AnnualReports.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("AnnualReport", "Reports");
        }
    }
}