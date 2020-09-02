using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XabugTracker.Helpers;
using XabugTracker.Models;

namespace XabugTracker.Controllers
{
    [Authorize]
    public class AssignmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectHelper projectHelper = new ProjectHelper();
        
        // POST: ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public ActionResult ManageRoles(List<string> userIds, string roleName)
        {
            if (userIds == null)
            {
                return RedirectToAction("RolesIndex");
            }

            foreach (var userId in userIds)
            {
                foreach (var role in roleHelper.ListUserRoles(userId).ToList())
                {
                    roleHelper.RemoveUserFromRole(userId, role);
                }

                if (!string.IsNullOrEmpty(roleName))
                {
                    roleHelper.AddUserToRole(userId, roleName);
                }
            }

            ViewBag.UserIds = new MultiSelectList(db.Users, "Id", "Email");

            ViewBag.RoleName = new SelectList(db.Roles, "Name", "Name");

            return View(db.Users.ToList());
        }
    }
}