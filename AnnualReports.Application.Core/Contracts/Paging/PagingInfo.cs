namespace AnnualReports.Application.Core.Contracts.Paging
{
    public class PagingInfo
    {
        public int PageIndex => PageNumber - 1;

        public int PageNumber { get; set; }

        public int Total { get; set; }
    }
}