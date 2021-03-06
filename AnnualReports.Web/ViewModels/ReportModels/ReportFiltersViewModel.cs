﻿using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AnnualReports.Web.ViewModels.ReportModels
{
    public class ReportFiltersViewModel
    {
        [Required(ErrorMessage = "Date is required.")]
        [UIHint("YearMonthDatePicker")]
        [Display(Name = "Please select date")]
        public DateTime? Date { get; set; } = DateTime.Now;

        [AllowedFileExtension(".xlsx")]
        [Required(ErrorMessage = "Please select file to process")]
        [Display(Name = "AuditorMaster excel file")]
        [UIHint("FileUpload")]
        public HttpPostedFileBase ExcelFile { get; set; }

        public List<MonthlyReportRule> MonthlyReportRules { get; set; }

        public List<MonthlyImportFundExceptionRule> MonthlyImportExceptionRule { get; set; }

        public string DateAsMonthYear
        {
            get { return Date.HasValue ? Date.Value.ToString("MM/yyyy") : ""; }
        }
    }
}