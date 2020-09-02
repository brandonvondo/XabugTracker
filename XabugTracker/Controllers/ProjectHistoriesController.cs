using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using XabugTracker.Models;

namespace XabugTracker.Controllers
{
    public class ProjectHistoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProjectHistories
        public ActionResult Index()
        {
            var projectHistories = db.ProjectHistories.Include(p => p.Project);
            return View(projectHistories.ToList());
        }

        // GET: ProjectHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectHistory projectHistory = db.ProjectHistories.Find(id);
            if (projectHistory == null)
            {
                return HttpNotFound();
            }
            return View(projectHistory);
        }

        // GET: ProjectHistories/Create
        public ActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ManagerId");
            return View();
        }

        // POST: ProjectHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,Message,Created")] ProjectHistory projectHistory)
        {
            if (ModelState.IsValid)
            {
                db.ProjectHistories.Add(projectHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ManagerId", projectHistory.ProjectId);
            return View(projectHistory);
        }

        // GET: ProjectHistories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectHistory projectHistory = db.ProjectHistories.Find(id);
            if (projectHistory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ManagerId", projectHistory.ProjectId);
            return View(projectHistory);
        }

        // POST: ProjectHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,Message,Created")] ProjectHistory projectHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ManagerId", projectHistory.ProjectId);
            return View(projectHistory);
        }

        // GET: ProjectHistories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectHistory projectHistory = db.ProjectHistories.Find(id);
            if (projectHistory == null)
            {
                return HttpNotFound();
            }
            return View(projectHistory);
        }

        // POST: ProjectHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectHistory projectHistory = db.ProjectHistories.Find(id);
            db.ProjectHistories.Remove(projectHistory);
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
