using Learny.Models;
using Learny.Settings;
using Learny.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public ActionResult NavigationLinks(int? courseId = null, int? moduleId = null, int? activityId = null)
        {
            string courseName="";
            string moduleName="";
            string activityName="";
            if (courseId != null)
            {
                var course = db.Courses.Where(c => c.Id == courseId).FirstOrDefault();
                if(course==null) return PartialView("_NavigationalLinksViewModel", new NavigationalLinksViewModel());

                courseName = course.FullCourseName;
            }
            if (moduleId != null)
            {
                var module = db.Modules.Where(m => m.Id == moduleId).FirstOrDefault();
                if (module == null) return PartialView("_NavigationalLinksViewModel", new NavigationalLinksViewModel());

                moduleName = module.Name;
                courseName = module.Course.FullCourseName;
                
                courseId = module.Course.Id;
            }
            if (activityId != null)
            {
                var activity = db.Activities.Where(a => a.Id == activityId).FirstOrDefault();
                if (activity == null) return PartialView("_NavigationalLinksViewModel", new NavigationalLinksViewModel());

                activityName = activity.Name;
                moduleName = activity.Module.Name;
                courseName = activity.Module.Course.FullCourseName;

                moduleId = activity.Module.Id;
                courseId = activity.Module.Course.Id;
            }
            
            var linkData = new NavigationalLinksViewModel()
            {
                CourseId = courseId,
                FullCourseName = courseName,
                ModuleId = moduleId,
                ModuleName = moduleName,
                ActivityId = activityId,
                ActivityName = activityName,
            };

            return PartialView("_NavigationalLinksViewModel", linkData);
        }

    }
}