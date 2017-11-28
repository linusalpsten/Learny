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
    public class StudentCourseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: StudentCourse
        public ActionResult Index()
        {
            ApplicationUser CurrentUser = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            Course course = db.Courses.Find(CurrentUser.CourseId);
            if (course == null)
            {
                return HttpNotFound();
            }

            StudentCourseViewModel ViewModel = new StudentCourseViewModel
            {
                Id = course.Id,
                Name = course.Name,
                CourseCode = course.CourseCode,
                Description = course.Description,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                Modules = course.Modules.OrderBy(m => m.StartDate).ToList(),
                Students = course.Students
            };

            return View(ViewModel);

        }

        // GET: StudentCourse/Details/5
        public ActionResult Details(int? id)
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
