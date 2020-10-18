using AnnualReports.Web.ViewModels.CommonModels;
using System.Collections.Generic;
using System.Linq;

namespace AnnualReports.Web.ViewModels.BarModels
{
    public class BarsListViewModel
    {
        public BarsListViewModel()
        {
            Data = Enumerable.Empty<BarDetailsViewModel>();
            UploadViewModel = new BarsUploadViewModel();
        }

        public YearFilterViewModel Filters { get; set; }

        public IEnumerable<BarDetailsViewModel> Data { get; set; }

        public BarsUploadViewModel UploadViewModel { get; set; }
    }
}