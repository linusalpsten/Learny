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
using Learny.Settings;

namespace Learny.Controllers
{
    [Authorize]
    public class CourseModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CourseModules
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Index()
        {
            return View(db.Modules.ToList());
        }

        // GET: CourseModules/Details/5
        [Authorize(Roles = RoleName.teacher + "," + RoleName.student)]
        public ActionResult Details(int? id)
        {
            //Student may not view module from other courses
            if (User.IsInRole(RoleName.student))
            {
                ApplicationUser currentUser = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                Course userCourse = db.Courses.Find(currentUser.CourseId);
                CourseModule userModule = db.Modules.Where(m => m.CourseId == userCourse.Id && m.Id == id).FirstOrDefault();
                if (userModule == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseModule courseModule = db.Modules.Find(id);
            if (courseModule == null)
            {
                return HttpNotFound();
            }

            var course = db.Courses.Find(courseModule.CourseId);
            if (course == null)
            {
                return HttpNotFound();
            }
            var module = new ModuleViewModel
            {
                Id = courseModule.Id,
                Name = courseModule.Name,
                Description = courseModule.Description,
                StartDate = courseModule.StartDate,
                EndDate = courseModule.EndDate,
                CourseId = courseModule.CourseId,
                FullCourseName = course.FullCourseName,
                Activities = courseModule.Activities.OrderBy(a => a.StartDate).ToList()
            };
            return View(module);
        }

        // GET: CourseModules/Create
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Create(int id)
        {
            var course = db.Courses.Where(c => c.Id == id).FirstOrDefault();
            if (course == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }



            var currentDateTime = DateTime.Now;
            var today = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day);

            var viewModel = new ModuleViewModel
            {
                FullCourseName = course.FullCourseName,
                CourseId = id,
                StartDate = today,
                EndDate = today
            };

            return View(viewModel);
        }

        // POST: CourseModules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleName.teacher)]
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


        // Egidio: below is Edit for Modules

        // GET: CourseModules/Edit/5
        [Authorize(Roles = RoleName.teacher)]
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
            var moduleView = new ModuleViewModel
            {
                Id = courseModule.Id,
                Name = courseModule.Name,
                StartDate = courseModule.StartDate,
                EndDate = courseModule.EndDate,
                Description = courseModule.Description,
                CourseId = courseModule.CourseId
            };
            return View(moduleView);
        }


        // POST: CourseModules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleName.teacher)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId")] ModuleViewModel moduleView)
        {
            if (ModelState.IsValid)
            {
                var courseModule = new CourseModule
                {
                    Id = moduleView.Id,
                    Name = moduleView.Name,
                    Description = moduleView.Description,
                    StartDate = moduleView.StartDate,
                    EndDate = moduleView.EndDate,
                    CourseId = moduleView.CourseId
                };
                db.Entry(courseModule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = moduleView.Id });
            }
            return View(moduleView);
        }

        // GET: CourseModules/Delete/5
        [Authorize(Roles = RoleName.teacher)]
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
        [Authorize(Roles = RoleName.teacher)]
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
