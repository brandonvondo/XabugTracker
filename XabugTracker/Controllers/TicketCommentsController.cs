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
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projectHelper = new ProjectHelper();

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketId,Comment")] TicketComment ticketComment)
        {
            var userId = User.Identity.GetUserId();
            var ticket = db.Tickets.Where(t => t.Id == ticketComment.TicketId).FirstOrDefault();
            if  (projectHelper.IsUserOnProject(userId, ticket.ProjectId))
            {
                if (ModelState.IsValid)
                {
                    ticketComment.UserId = User.Identity.GetUserId();
                    ticketComment.Created = DateTime.Now;
                    db.TicketComments.Add(ticketComment);
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Tickets", new { id = ticketComment.TicketId });
                }
            }
            return RedirectToAction("Index", "Tickets");
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,Comment,Created")] TicketComment ticketComment)
        {
            if(User.Identity.GetUserId() == ticketComment.UserId)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(ticketComment).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index", "Tickets");
        }

        // POST: TicketComments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (User.Identity.GetUserId() == ticketComment.UserId || User.IsInRole("Project Manager"))
            {
                db.TicketComments.Remove(ticketComment);
                db.SaveChanges();
            }

            return RedirectToAction("Dashboard", "Tickets", new { id = ticketComment.TicketId });
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
