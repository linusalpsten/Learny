using Learny.Settings;
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
                
    }
}