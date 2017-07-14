namespace AnnualReports.Application.Core.Contracts.BarEntities
{
    public class BarUploadEntity
    {
        public int Id { get; set; }

        public string BarNumber { get; set; }

        public string DisplayName { get; set; }

        public int Year { get; set; }

        public string MapToBarNumber { get; set; }

        public bool IsActive { get; set; }

        public int? Period { get; set; }
    }
}