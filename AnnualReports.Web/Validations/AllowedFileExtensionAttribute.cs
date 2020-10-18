using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AnnualReports.Web.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllowedFileExtensionAttribute : ValidationAttribute
    {
        public string[] AllowedFileExtensions { get; private set; }

        public AllowedFileExtensionAttribute(params string[] allowedFileExtensions)
        {
            AllowedFileExtensions = allowedFileExtensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as HttpPostedFileBase;
            if (file != null)
            {
                if (!AllowedFileExtensions.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase)))
                {
                    return new ValidationResult(string.Format("The allowed file extensions for \"{0}\" are : {1}", validationContext.DisplayName, string.Join(", ", AllowedFileExtensions)));
                }
            }
            return null;
        }
    }
}