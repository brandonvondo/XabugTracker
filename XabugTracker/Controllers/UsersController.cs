using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XabugTracker.Helpers;
using XabugTracker.Models;

namespace XabugTracker.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: ManageUserRoles
        public ActionResult ManageUserRoles(string id)
        {
            var userRole = roleHelper.ListUserRoles(id).FirstOrDefault();
            ViewBag.RoleName = new SelectList(db.Roles, "Name", "Name", userRole);
            return View(db.Users.Find(id));
        }

        // POST: ManageUserRoles

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ManageUserRoles(string id, string roleName)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            foreach(var role in roleHelper.ListUserRoles(id))
            {
                roleHelper.RemoveUserFromRole(id, role);
            }

            if (!string.IsNullOrEmpty(roleName))
            {
                roleHelper.AddUserToRole(id, roleName);
            }

            return RedirectToAction("ManageUserRoles", new { id });
        }
        }
    }