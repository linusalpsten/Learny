using Learny.Models;
using Learny.Settings;
using Learny.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace Learny.Controllers
{
    [Authorize(Roles = RoleName.teacher)]
    public class TeacherController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //
        // GET
        public ActionResult Create()
        {
            var viewModel = new TeacherCreateViewModel { };
            return View("Manage",viewModel);
        }

        //
        // POST
        [HttpPost]
        [Authorize(Roles = RoleName.teacher)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeacherCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email is already used by other users
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "En användare med den e-post adressen finns redan");
                    return View("Manage");
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

                if (result.Succeeded)
                {
                    var existingUser = userManager.FindByName(model.Email);
                    if (!userManager.IsInRole(existingUser.Id, RoleName.teacher))
                    {
                        userManager.AddToRole(existingUser.Id, RoleName.teacher);
                    }


                    var feedbackMessage = "Läraren har lagts till";
                    var teacherViewModel = new TeacherViewModel(existingUser);
                    addFeedbackToTempData(feedbackMessage, teacherViewModel);

                    return View("Manage", new TeacherCreateViewModel());
                }

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
                // Add swedish error message
                var resultModified = new IdentityResult(errorsInSwedish);
                AddErrors(resultModified);
            }

            // If we got this far, something failed, redisplay form
            return View("Manage", model);
        }

        public ActionResult Details(string email)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var teacher = userManager.FindByEmail(email);
            var teacherViewModel = new TeacherViewModel(teacher);

            return View(teacherViewModel);
        }

        public ActionResult Edit(string email)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var teacher = userManager.FindByEmail(email);
            var teacherViewModel = new TeacherViewModel(teacher);

            return View(teacherViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TeacherViewModel teacherViewModel)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (ModelState.IsValid)
            {
                //Check that email is not used by another user
                var teacher = userManager.FindByEmail(teacherViewModel.Email);
                if (teacher != null && teacherViewModel.Id != teacher.Id)
                {
                    ModelState.AddModelError("Email", "E-post adressen används redan");
                    return View(teacherViewModel);
                }

                // Get the existing user from the db
                var teacherToUpdate = userManager.FindById(teacherViewModel.Id);

                // Update it with the values from the view model
                teacherToUpdate.Name = teacherViewModel.Name;
                teacherToUpdate.UserName = teacherViewModel.Email;
                teacherToUpdate.Email = teacherViewModel.Email;

                // Apply the changes if any to the db
                userManager.Update(teacherToUpdate);

                //Get updated user from database
                var updatedTeacher = userManager.FindById(teacherViewModel.Id);
                var updatedTeacherViewModel = new TeacherViewModel
                {
                    Name = updatedTeacher.Name,
                    Email = updatedTeacher.Email,
                };

                return View("Details", updatedTeacherViewModel);
            }

            return View(teacherViewModel);

        }

        public void addFeedbackToTempData(string message, TeacherViewModel teacher)
        {
            TempData["FeedbackMessage"] = message;
            TempData["FeedbackData"] = teacher;
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