using Learny.Models;
using Learny.ViewModels;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;

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
                FullCourseName = course.FullCourseName,
                Description = course.Description,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                Modules = course.Modules.OrderBy(m => m.StartDate).ToList(),
                Students = course.Students
            };

            return View(ViewModel);

        }

        // GET: StudentCourse/Module/5
        public ActionResult Module(int? id)
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

        // GET: StudentCourse/Activity/5
        public ActionResult Activity(int? id)
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
            var activityViewModel = new StudentModuleActivity
            {
                Id = moduleActivity.Id,
                Name = moduleActivity.Name,
                Description = moduleActivity.Description,
                StartDate = moduleActivity.StartDate,
                EndDate = moduleActivity.EndDate,
                ActivityTypeName = moduleActivity.ActivityType.Name
            };
            return View(activityViewModel);
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
