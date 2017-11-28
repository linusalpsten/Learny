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
    [Authorize]
    public class CourseModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CourseModules
        public ActionResult Index()
        {
            return View(db.Modules.ToList());
        }

        // GET: CourseModules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseModule courseModule = db.Modules.Find(id);
            courseModule.Activities = courseModule.Activities.OrderBy(a => a.StartDate).ToList();
            if (courseModule == null)
            {
                return HttpNotFound();
            }
            return View(courseModule);
        }

        // GET: CourseModules/Create
        public ActionResult Create(int id)
        {
            var viewModel = new ModuleViewModel
            {
                CourseId = id,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            return View(viewModel);
        }

        // POST: CourseModules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] ModuleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var courseModule = new CourseModule
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    CourseId = viewModel.CourseId
                };

                db.Modules.Add(courseModule);
                db.SaveChanges();
                return RedirectToAction("Details", "Courses", new { id = viewModel.CourseId });
            }

            return View(viewModel);
        }

        // GET: CourseModules/Edit/5
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

        // POST: CourseModules/Edit/5
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

        // GET: CourseModules/Delete/5
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

        // POST: CourseModules/Delete/5
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
