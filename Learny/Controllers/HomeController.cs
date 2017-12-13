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
            var documents = new List<Document>();
            if (courseId != null) documents = db.Documents.Where(d => d.CourseId == courseId).ToList();
            if (moduleId != null) documents = db.Documents.Where(d => d.CourseModuleId == moduleId).ToList();
            if (activityId != null) documents = db.Documents.Where(d => d.ModuleActivityId == activityId).ToList();

            var linkData = new NavigationalLinksViewModel()
            {
                CourseId = courseId,
                ModuleId = moduleId,
                ActivityId = activityId,
            };

            return PartialView("_NavigationalLinksPartial", linkData);
        }

    }
}