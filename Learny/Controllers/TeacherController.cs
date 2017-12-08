using Learny.Models;
using Learny.Settings;
using Learny.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Learny.Controllers
{
    public class TeacherController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //
        // GET: /Account/CreateTeacher
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Create()
        {
            var viewModel = new TeacherViewModel { };
            return View("Manage",viewModel);
        }

        //
        // POST: /Account/CreateTeacher
        [HttpPost]
        [Authorize(Roles = RoleName.teacher)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email is already used by other users
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "En användare med den e-post adressen finns redan");
                    return View();
                }

                var user = new ApplicationUser
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email
                };

                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var result = userManager.Create(user, model.Password);

                var errorsInSwedish = new List<string>();

                foreach (var error in result.Errors)
                {
                    if (error.Substring(0, error.IndexOf(" ")) == "Passwords")
                    {
                        errorsInSwedish.Add("Lösenord måste ha minst en icke bokstav, en siffra, en versal('A' - 'Z') och bestå av minst 6 tecken.");
                    }
                    else
                    {
                        errorsInSwedish.Add(error);
                    }
                }

                if (result.Succeeded)
                {
                    var existingUser = userManager.FindByName(model.Email);
                    if (!userManager.IsInRole(existingUser.Id, RoleName.teacher))
                    {
                        userManager.AddToRole(existingUser.Id, RoleName.teacher);
                    }


                    TempData["Feedback"] = "Lärare: " + model.Name + " med e-posten: " + model.Email + " har lagts till";
                    return RedirectToAction("Create");
                }
                // Add swedish error message
                var resultModified = new IdentityResult(errorsInSwedish);
                AddErrors(resultModified);
            }

            // If we got this far, something failed, redisplay form
            return View("Manage", model);
        }

        public ActionResult ListTeachers()
        {
            var role = db.Roles.SingleOrDefault(m => m.Name == RoleName.teacher);
            var allTeachers = db.Users.Where(u => u.Roles.Any(r => r.RoleId == role.Id)).OrderBy(t => t.Name).ToList();
            
            List<TeacherViewModel> teachers = new List<TeacherViewModel>();

            foreach (var teacher in allTeachers)
            {
                teachers.Add(new TeacherViewModel
                {
                    Name = teacher.Name,
                    Email = teacher.Email
                });
            }

            return PartialView("_TeachersPartial", teachers);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}