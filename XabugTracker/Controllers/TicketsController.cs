using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XabugTracker.Helpers;
using XabugTracker.Models;

namespace XabugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projectHelper = new ProjectHelper();
        private TicketHelper ticketHelper = new TicketHelper();
        private UserRolesHelper userRolesHelper = new UserRolesHelper();
        private HistoryHelper historyHelper = new HistoryHelper();

        // GET: Tickets
        public ActionResult Index()
        {
            var userRole = userRolesHelper.ListUserRole();
            var returnView = new List<Ticket>();
            var userId = User.Identity.GetUserId();

            switch (userRole)
            {
                case "Admin":
                    returnView = db.Tickets.ToList();
                    break;

                case "Submitter":
                case "Developer":
                case "Project Manager":
                default:
                    returnView = ticketHelper.ListUserTickets(User.Identity.GetUserId());
                    break;
                
            }
            return View(returnView);
        }

        public ActionResult GetProjectTickets()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var ticketList = new List<Ticket>();
            ticketList = user.Projects.SelectMany(p => p.Tickets).ToList();
            return View("Index", ticketList);
        }

        public ActionResult GetAssignedTickets()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var ticketList = new List<Ticket>();
            ticketList = user.Projects.SelectMany(p => p.Tickets.Where(t => t.DeveloperId == userId)).ToList();
            return View("Index", ticketList);
        }

        public ActionResult GetSubmittedTickets()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var ticketList = new List<Ticket>();
            ticketList = user.Projects.SelectMany(p => p.Tickets.Where(t => t.SubmitterId == userId)).ToList();
            return View("Index", ticketList);
        }



        // GET: Tickets/Dashboard/5
        public ActionResult Dashboard(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            List<ApplicationUser> projUse = ticket.Project.Users.ToList();
            if(projectHelper.IsUserOnProject(User.Identity.GetUserId(), ticket.ProjectId))
            {
                ViewBag.DeveloperIds = new SelectList(userRolesHelper.ProjectUsersInRole("Developer", projUse), "Id", "FullName");
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
                return View(ticket);
            }
            return RedirectToAction("Index", "Tickets");
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create([Bind(Include = "Id,ProjectId,TicketPriorityId,TicketTypeId,Issue,IssueDescription")] Ticket ticket)
        {
            var userId = User.Identity.GetUserId();
            var onProject = projectHelper.IsUserOnProject(userId, ticket.ProjectId);
            if (onProject)
            {
                if (ModelState.IsValid)
                {
                    ticket.TicketStatusId = db.TicketStatuses.Where(ts => ts.Name == "Open").FirstOrDefault().Id;
                    ticket.SubmitterId = userId;
                    ticket.Created = DateTime.Now;
                    db.Tickets.Add(ticket);
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Tickets", new { id = ticket.ProjectId });
                }
            }
            
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Project Manager")]
        //POST: Tickets/Dashboard modal AssignDev/6
        public ActionResult AssignDev(string DeveloperIds, string oldDev, string Id)
        {
            if (oldDev == "")
            {
                oldDev = null;
            }
            int idLook = Int16.Parse(Id);
            Ticket ticket = db.Tickets.Where(t => t.Id == idLook).FirstOrDefault();
            if (ticket.Project.ManagerId == User.Identity.GetUserId())
            {
                if (oldDev == "")
                {
                    ticketHelper.ManageTicketNotifications(null, DeveloperIds, idLook);
                }
                else
                {
                    ticketHelper.ManageTicketNotifications(oldDev, DeveloperIds, idLook);
                }
                historyHelper.DeveloperUpdate(oldDev, DeveloperIds, idLook);
                ticket.DeveloperId = DeveloperIds;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Dashboard", new { id = Id });
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,TicketPriorityId,TicketStatusId,TicketTypeId,SubmitterId,DeveloperId,Issue,IssueDescription,Created,Updated,IsResolved,IsArchived")] Ticket ticket)
        {
            var userId = User.Identity.GetUserId();
            
            if(ticket.SubmitterId == userId || ticket.DeveloperId == userId)
            {
                if (ModelState.IsValid)
                {
                    ticket.Updated = DateTime.Now;
                    var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                    var newTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                    historyHelper.ManageHistories(oldTicket, newTicket);
                    ticketHelper.ManageTicketNotifications(oldTicket, newTicket);
                    return RedirectToAction("Dashboard", new { id = newTicket.Id });
                }
            }
            

            return RedirectToAction("Index","Tickets");
        }

        public ContentResult ManageNotifications(FormCollection formCollection)
        {
            var notifId = formCollection["Id"];
            var purposeName = formCollection["Purpose"];
            int notifLook = Int16.Parse(notifId);
            string message;
            if (purposeName == "delete")
            {
                ticketHelper.DeleteNotification(notifLook);
                message = "Your notification has been deleted.";
            }
            else
            {
                ticketHelper.ReadNotification(notifLook);
                message = "Your notification has been marked as read and can be reread/deleted in your profile";
            }

            return Content(message);
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
