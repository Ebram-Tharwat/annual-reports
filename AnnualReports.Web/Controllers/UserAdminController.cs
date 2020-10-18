using AnnualReports.Domain.Core.AnnualReportsDbModels;
using AnnualReports.Web.Models;
using AnnualReports.Web.ViewModels.Admin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace AnnualReports.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserAdminController : Controller
    {
        private int pageSize = int.Parse(ConfigurationManager.AppSettings["pageSize"].ToString());
        // GET: UserAdmin

        public UserAdminController()
        {
        }

        public UserAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //
        // GET: /Users/
        /// <summary>
        /// users admin index page
        /// </summary>
        /// <param name="page">page number</param>
        /// <returns>index page html</returns>
        public async Task<ActionResult> Index(int? page)
        {
            return View();
        }

        /// <summary>
        /// list users based on parameters
        /// </summary>
        /// <param name="page">page number</param>
        /// <param name="searchVal">users name contains that value</param>
        /// <returns></returns>
        public ActionResult GetUsers(int? page, string searchVal)
        {
            int pageNumber = (page ?? 1);
            var query = UserManager.Users.Where(u => 1 == 1);
            if (!string.IsNullOrWhiteSpace(searchVal))
            {
                query = query.Where(u => u.UserName.Contains(searchVal));
            }

            ViewBag.searchVal = searchVal;

            return PartialView(query.OrderByDescending(u => u.FullName).ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Users/Details/5
        /// <summary>
        /// user details page
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>user details page html</returns>
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        /// <summary>
        /// create new user page
        /// </summary>
        /// <returns>create new user page html</returns>
        public async Task<ActionResult> Create()
        {
            //MeetingsDataContext db = new MeetingsDataContext();

            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        /// <summary>
        /// insert new user into database
        /// </summary>
        /// <param name="userViewModel">user data</param>
        /// <param name="selectedRoles">user roles</param>
        /// <returns>if success go to users list page if there is error stay on the same page</returns>
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { EmailConfirmed = true, UserName = userViewModel.UserName, Email = userViewModel.Email, FullName = userViewModel.FullName, PhoneNumber = userViewModel.PhoneNumber, };
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    return View();
                }
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
        }

        //
        // GET: /Users/Edit/1
        /// <summary>
        /// edit user page
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>edit user page html</returns>
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                bool lockedout = await UserManager.IsLockedOutAsync(user.Id); // Check for lockout
                // UserManager.ResetAccessFailedCountAsync(user.Id); // Clear failed count after success
                //UserManager.AccessFailedAsync(user.Id); // Record a failure (this will lockout if enabled)
                //UserManager.SetLockoutEnabled(user.Id, enabled) // Enables or disables lockout for a user

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                return View(new EditUserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    LockedOut = lockedout,

                    RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                    {
                        Selected = userRoles.Contains(x.Name),
                        Text = x.Name,
                        Value = x.Name
                    })
                });
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //
        // POST: /Users/Edit/5
        /// <summary>
        /// update user information in database
        /// </summary>
        /// <param name="editUser">user information</param>
        /// <param name="selectedRole">user roles</param>
        /// <returns>if success go to users index page if there is error stay in the same page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,FullName,PhoneNumber, UserName,LockedOut")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                user.UserName = editUser.UserName;
                user.Email = editUser.Email;
                user.FullName = editUser.FullName;
                user.PhoneNumber = editUser.PhoneNumber;

                if (editUser.LockedOut)
                {
                    UserManager.SetLockoutEnabled(user.Id, true);
                    int maxcount = int.Parse(ConfigurationManager.AppSettings["MaxFailedAccessAttemptsBeforeLockout"].ToString());
                    for (int i = 0; i < maxcount; i++)
                    {
                        await UserManager.AccessFailedAsync(user.Id); // Record a failure (this will lockout if enabled)
                    }
                }
                else
                {
                    UserManager.SetLockoutEnabled(user.Id, false);
                    await UserManager.ResetAccessFailedCountAsync(user.Id);
                }

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());

                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "حدث خطأ ما.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        /// <summary>
        /// delete user page
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>delete user confirm page html</returns>
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        /// <summary>
        /// delete user from database
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>if success return to users list page if there is error stay on the same page</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}