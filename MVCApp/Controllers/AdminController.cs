using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using MVCApp.ViewModels;
using AutoMapper;
using Microsoft.AspNet.Identity;
using MVCApp.Models;
using MVCApp.Repository;
using PagedList;

namespace MVCApp.Controllers
{
    [Authorize(Roles = "Admin, Candidate")]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly CandidateBadgeRepository _badgeRepository;

        public AdminController()
        {
            if (System.Web.HttpContext.Current.Session["candidateRepo"] == null)
            {
                _badgeRepository = new CandidateBadgeRepository();
                System.Web.HttpContext.Current.Session["candidateRepo"] = _badgeRepository;
            }
            else
            {
                _badgeRepository = (CandidateBadgeRepository) System.Web.HttpContext.Current.Session["candidateRepo"];
            }
        }

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

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
        public ActionResult Index(string searchStringUserNameOrEmail, string currentFilter, int? page)
        {
            try
            {
                int intPage = 1;
                int intPageSize = 5;
                int intTotalPageCount = 0;

                if (searchStringUserNameOrEmail != null)
                {
                    intPage = 1;
                }
                else
                {
                    if (currentFilter != null)
                    {
                        searchStringUserNameOrEmail = currentFilter;
                        intPage = page ?? 1;
                    }
                    else
                    {
                        searchStringUserNameOrEmail = "";
                        intPage = page ?? 1;
                    }
                }

                ViewBag.CurrentFilter = searchStringUserNameOrEmail;

                var intSkip = (intPage - 1) * intPageSize;

                intTotalPageCount = UserManager.Users
                    .Count(x => x.UserName.Contains(searchStringUserNameOrEmail));

                var result = UserManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameOrEmail))
                    .OrderBy(x => x.UserName)
                    .Skip(intSkip)
                    .Take(intPageSize)
                    .ToList();

                var users = Mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<ExpandedUserViewModel>>(result).ToList();

                foreach (var user in users)
                {
                    var role = UserManager.GetRoles(user.Id);
                    user.Role = role.ElementAt(0);
                }
                // Set the number of pages
                var usersPageList =
                    new StaticPagedList<ExpandedUserViewModel>
                    (
                        users, intPage, intPageSize, intTotalPageCount
                    );

                return View(usersPageList);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, @"Error: " + ex);
                var users= new List<ExpandedUserViewModel>();
                return View(users.ToPagedList(1, 25));
            }
        }

         public ActionResult MakeAdmin(string userName)
        {
            var user = UserManager.FindByName(userName);
            user.Roles.Clear();
            UserManager.Update(user);
            UserManager.AddToRole(user.Id, "Admin");
            return RedirectToAction("Index");
        }

        [OverrideAuthorization]
        [Authorize(Roles = "Candidate")]
        public JsonResult SessionHasChanges()
        {
            if (_badgeRepository.HasChanges())
            {
                return Json(new { sessionHasChanged = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { sessionHasChanged = false }, JsonRequestBehavior.AllowGet);
        }

        [OverrideAuthorization]
        [Authorize(Roles = "Candidate")]
        public ActionResult SessionChanges()
        {
            var modifiedChanges = _badgeRepository.GetModifiedBadges();
            return View(modifiedChanges);
        }

        [OverrideAuthorization]
        [Authorize(Roles = "Candidate")]
        public ActionResult SaveSessionChanges()
        {
            _badgeRepository.SaveChanges();
            return RedirectToAction("Index", "Badge");
        }

        [OverrideAuthorization]
        [Authorize(Roles = "Candidate")]
        public ActionResult CancelSessionChanges()
        {
            _badgeRepository.Cancel();
            return RedirectToAction("Index", "Badge");
        }

    }
}