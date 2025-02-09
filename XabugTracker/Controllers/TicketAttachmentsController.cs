﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XabugTracker.Helpers;
using XabugTracker.Models;

namespace XabugTracker.Controllers
{
    [Authorize]
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private FileStamper fileStamper = new FileStamper();
        private ProjectHelper projectHelper = new ProjectHelper();

        // GET: TicketAttachments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketId,FileName,Description")] TicketAttachment ticketAttachment, HttpPostedFileBase file)
        {
            var userId = User.Identity.GetUserId();
            var ticket = db.Tickets.Where(t => t.Id == ticketAttachment.TicketId).FirstOrDefault();
            if (projectHelper.IsUserOnProject(userId, ticket.ProjectId))
            {
                if (ModelState.IsValid)
                {
                    ticketAttachment.Created = DateTime.Now;
                    ticketAttachment.UserId = User.Identity.GetUserId();
                    if (file == null)
                    {
                        TempData["Error"] = "You must supply a file!";
                        return RedirectToAction("Dashboard", "Tickets", new { id = ticketAttachment.Id });
                    }

                    if (ImageUploadValidator.IsWebFriendlyImage(file) || FileUploadValidator.IsWebFriendlyFile(file))
                    {
                        var fileName = fileStamper.MakeUnique(file.FileName);
                        ticketAttachment.FileName = fileName;
                        file.SaveAs(Path.Combine(Server.MapPath("~/UserUploads/"), fileName));
                        ticketAttachment.FilePath = "/UserUploads/" + fileName;
                    }

                    db.TicketAttachments.Add(ticketAttachment);
                    db.SaveChanges();
                    return RedirectToAction("Dashboard", "Tickets", new { id = ticketAttachment.TicketId });
                }
                TempData["Error"] = "The model was invalid.";
                return RedirectToAction("Dashboard", "Tickets", new { id = ticketAttachment.TicketId });
            }

            return RedirectToAction("Dashboard", "Tickets", new { id = ticketAttachment.TicketId });
        }

        // GET: TicketAttachments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketId,UserId,FilePath,Description,Created")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "SubmitterId", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
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
