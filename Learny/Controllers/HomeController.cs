using Learny.Models;
using Learny.Settings;
using Learny.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace Learny.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            if (User.IsInRole(RoleName.student))
            {
                return RedirectToAction("Details", "Courses");
            }
            if (User.IsInRole(RoleName.teacher))
            {
                return RedirectToAction("Index", "Courses");
            }

            return RedirectToAction("Login", "AccountController");
        }

        public enum IdType { Course, Module, Activity };

        public ActionResult NavigationLinks(IdType idType, int id)
        {
            int? courseId=null;
            int? moduleId=null;
            int? activityId=null;
            string courseName="";
            string moduleName="";
            string activityName="";
            if (idType == IdType.Course)
            {
                var course = db.Courses.Where(c => c.Id == id).FirstOrDefault();
                if (course != null)
                {
                    courseName = course.FullCourseName;

                    courseId = id;
                }
            }
            if (idType == IdType.Module)
            {
                var module = db.Modules.Where(m => m.Id == id).FirstOrDefault();
                if (module != null)
                {
                    moduleName = module.Name;
                    courseName = module.Course.FullCourseName;

                    moduleId = id;
                    courseId = module.Course.Id;
                }
            }
            if (idType == IdType.Activity)
            {
                var activity = db.Activities.Where(a => a.Id == id).FirstOrDefault();
                if (activity != null)
                {
                    activityName = activity.Name;
                    moduleName = activity.Module.Name;
                    courseName = activity.Module.Course.FullCourseName;

                    activityId = id;
                    moduleId = activity.Module.Id;
                    courseId = activity.Module.Course.Id;
                }
            }
            
            var linkData = new NavigationLinksViewModel()
            {
                CourseId = courseId,
                FullCourseName = courseName,
                ModuleId = moduleId,
                ModuleName = moduleName,
                ActivityId = activityId,
                ActivityName = activityName,
            };

            return PartialView("_NavigationLinksPartial", linkData);
        }

    }
}