using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XabugTracker.Models;
using Microsoft.AspNet.Identity;
using XabugTracker.Helpers;

namespace XabugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserHelper userHelper = new UserHelper();
        private ProjectHelper projectHelper = new ProjectHelper();
        private HistoryHelper historyHelper = new HistoryHelper();
        private UserRolesHelper userRoleHelper = new UserRolesHelper();

        // GET: Projects
        public ActionResult Index()
        {
            var userRole = userRoleHelper.ListUserRole();
            var returnView = new List<Project>();
            var userId = User.Identity.GetUserId();

            switch (userRole) { 
                case "Admin":
                returnView = db.Projects.ToList();
                    break;

            case "Submitter":
            case "Developer":
            case "Project Manager":
                returnView = projectHelper.ListUserProjects(User.Identity.GetUserId());
            break;
            }
                
            return View(returnView);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            if(projectHelper.IsUserOnProject(User.Identity.GetUserId(), project.Id) || User.IsInRole("Admin"))
            {
                var userIds = userRoleHelper.UsersNotInRole("Project Manager");
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
                ViewBag.Users = userIds;
                return View(project);
            }

            return RedirectToAction("Index", "Projects");
        }

        [HttpPost]
        public ContentResult ManageProjectMembers(FormCollection formCollection)
        {
            var userId = formCollection["UseId"];
            var user = userHelper.GetUserById(userId);
            var projId = formCollection["ProjId"];
            int projLook = Int16.Parse(projId);
            var switchBool = formCollection["SwitchBool"];
            string message = "";
            Project project = projectHelper.FindProjectById(projLook);
            if (project.ManagerId == userId || User.IsInRole("Admin"))
            {
                if (switchBool == "true")
                {
                    projectHelper.AddUserToProject(userId, projLook);
                    message = user.FullName + " has been added to " + project.Name;
                    string PHmessage = user.FullName + " was added to " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                    historyHelper.CreateProjectHistory(projLook, PHmessage);
                }
                else
                {
                    bool removeWork = projectHelper.RemoveUserFromProject(userId, projLook);
                    if (removeWork)
                    {
                        message = user.FullName + " has been removed from " + project.Name;
                        string PHmessage = user.FullName + " was removed from " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy h tt");
                        historyHelper.CreateProjectHistory(projLook, PHmessage);
                    }
                    else
                    {
                        message = "There was an error removing " + user.FullName + " from " + project.Name;
                    }
                }
            }
            
            return Content(message);
        }


        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,Created,IsArchived")] Project project, string pmId)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser creator = db.Users.Find(User.Identity.GetUserId());
                var user = db.Users.Find(pmId);
                project.Users.Add(user);
                project.ManagerId = pmId;
                project.Created = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                string PHmessage = creator.FullName + " has created " + project.Name + " at " + DateTime.Now.ToString("MMM dd, yyyy H tt");
                historyHelper.CreateProjectHistory(project.Id, PHmessage);
                PHmessage = "Project Manager " + user.FullName + " has been assigned to be the project manager";
                historyHelper.CreateProjectHistory(project.Id, PHmessage);
                return RedirectToAction("Details", "Projects", new { id = project.Id });
            }

            return View(project);
        }


        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
