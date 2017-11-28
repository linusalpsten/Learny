using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Learny.Models;
using Learny.ViewModels;

namespace Learny.Controllers
{
    public class StudentCourseModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StudentCourseModules
        public ActionResult OldIndex()
        {
            return View(db.Modules.ToList());
        }

        // GET: StudentCourseModules/Details/5
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseModule courseModule = db.Modules.Find(id);
            if (courseModule == null)
            {
                return HttpNotFound();
            }
            var courseModuleViewModel = new StudentCourseModuleViewModel
            {
                Id = courseModule.Id,
                Name = courseModule.Name,
                Description = courseModule.Description,
                StartDate = courseModule.StartDate,
                EndDate = courseModule.EndDate,
                Activities = db.Activities.Where(a => a.CourseModuleId == courseModule.Id).OrderBy(a => a.StartDate).ToList()
            };
            return View(courseModuleViewModel);
        }

        // GET: StudentCourseModules/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StudentCourseModules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] CourseModule courseModule)
        {
            if (ModelState.IsValid)
            {
                db.Modules.Add(courseModule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseModule);
        }

        // GET: StudentCourseModules/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseModule courseModule = db.Modules.Find(id);
            if (courseModule == null)
            {
                return HttpNotFound();
            }
            return View(courseModule);
        }

        // POST: StudentCourseModules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] CourseModule courseModule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseModule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(courseModule);
        }

        // GET: StudentCourseModules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseModule courseModule = db.Modules.Find(id);
            if (courseModule == null)
            {
                return HttpNotFound();
            }
            return View(courseModule);
        }

        // POST: StudentCourseModules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseModule courseModule = db.Modules.Find(id);
            db.Modules.Remove(courseModule);
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
