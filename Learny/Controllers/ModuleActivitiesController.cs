using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Learny.Models;

namespace Learny.Controllers
{
    [Authorize]
    public class ModuleActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ModuleActivities
        public ActionResult Index()
        {
            var activities = db.Activities.Include(m => m.ActivityType);
            return View(activities.ToList());
        }

        // GET: ModuleActivities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleActivity moduleActivity = db.Activities.Find(id);
            if (moduleActivity == null)
            {
                return HttpNotFound();
            }
            return View(moduleActivity);
        }

        // GET: ModuleActivities/Create
        public ActionResult Create()
        {
            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name");
            return View();
        }

        // POST: ModuleActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseModuleId,ActivityTypeId")] ModuleActivity moduleActivity)
        {
            if (ModelState.IsValid)
            {
                db.Activities.Add(moduleActivity);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", moduleActivity.ActivityTypeId);
            return View(moduleActivity);
        }

        // GET: ModuleActivities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleActivity moduleActivity = db.Activities.Find(id);
            if (moduleActivity == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", moduleActivity.ActivityTypeId);
            return View(moduleActivity);
        }

        // POST: ModuleActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseModuleId,ActivityTypeId")] ModuleActivity moduleActivity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(moduleActivity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", moduleActivity.ActivityTypeId);
            return View(moduleActivity);
        }

        // GET: ModuleActivities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleActivity moduleActivity = db.Activities.Find(id);
            if (moduleActivity == null)
            {
                return HttpNotFound();
            }
            return View(moduleActivity);
        }

        // POST: ModuleActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModuleActivity moduleActivity = db.Activities.Find(id);
            db.Activities.Remove(moduleActivity);
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
