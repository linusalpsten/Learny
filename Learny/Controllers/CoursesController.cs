using Learny.ViewModels;
using Learny.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace Learny.Models
{
    [Authorize]
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Courses
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Index()
        {
            var courses = db.Courses.ToList();
            List<CourseViewModel> viewModels = new List<CourseViewModel>();
            foreach (var course in courses)
            {
                viewModels.Add(populateCourseVM(course));
            }

            var sortedViewModel = viewModels.OrderBy(v => v.StartDate);
            return View(sortedViewModel);
        }

        private CourseViewModel populateCourseVM(Course course)
        {
            CourseViewModel viewModel = new CourseDetailsViewModel
            {
                Id = course.Id,
                Name = course.Name,
                CourseCode = course.CourseCode,
                FullCourseName = course.FullCourseName,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                Modules = course.Modules.OrderBy(m => m.StartDate).ToList()
            };
            return viewModel;
        }

        // GET: Courses/Details/5
        [Authorize(Roles = RoleName.teacher +","+  RoleName.student)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            CourseDetailsViewModel viewModel = (CourseDetailsViewModel)populateCourseVM(course);
            viewModel.Students = course.Students;
            viewModel.Description = course.Description;
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        // GET: Courses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CourseCode,Description,StartDate,EndDate")] CourseCreateViewModel courseView)
        {
            if (ModelState.IsValid)
            {
                if (db.Courses.Any(c => c.CourseCode == courseView.CourseCode))
                {
                    ModelState.AddModelError("CourseCode", "Kurskoden finns redan");
                    return View(courseView);
                }
                var course = new Course
                {
                    Id = courseView.Id,
                    Name = courseView.Name,
                    CourseCode = courseView.CourseCode,
                    Description = courseView.Description,
                    StartDate = courseView.StartDate,
                    EndDate = courseView.EndDate
                };

                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseView);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CourseCode,Description,StartDate,EndDate")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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
