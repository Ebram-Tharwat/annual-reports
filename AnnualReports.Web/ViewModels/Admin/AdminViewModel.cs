using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AnnualReports.Web.ViewModels.Admin
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Role name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// edit user page view model
    /// </summary>
    public class EditUserViewModel
    {
        /// <summary>
        /// user id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// user email
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// user name
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        /// <summary>
        /// user phone number
        /// </summary>
        [Display(Name = "Mobile number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// is this user lockedout
        /// </summary>
        [Display(Name = "Is locked out?")]
        public bool LockedOut { get; set; }

        /// <summary>
        /// roles that user part of
        /// </summary>
        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}