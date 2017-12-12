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
    [Authorize]
    public class StudentController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        

        [Authorize(Roles = RoleName.teacher)]
        public ActionResult CreateStudentFromNavBar()
        {
            return RedirectToAction("Create");
        }
        
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                var allCourses = db.Courses.ToList();
                var viewModel = new StudentCreateViewModel
                {
                    Courses = allCourses,
                    CourseSelected = false
                };

                return View(viewModel);
            }

            var course = db.Courses.Find(id);
            if (course == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var viewModelSelectedCourse = new StudentCreateViewModel
            {
                AttendingCourse = course.FullCourseName,
                CourseId = course.Id,
                CourseSelected = true
            };

            return View(viewModelSelectedCourse);

        }
        
        [HttpPost]
        [Authorize(Roles = RoleName.teacher)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentCreateViewModel model)
        {
            var allCourses = db.Courses.ToList();
            if (ModelState.IsValid)
            {
                // Check if email is already used by other users
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "En användare med den e-post adressen finns redan");
                    model.Courses = allCourses;
                    return View("Create", model);
                }

                var user = new ApplicationUser
                {
                    CourseId = model.CourseId,
                    // CourseName = model.CourseCode,
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email
                };

                var errorsInSwedish = new List<string>();

                var userStore = new UserStore<ApplicationUser>(db);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var result = userManager.Create(user, model.Password);

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
                    if (!userManager.IsInRole(existingUser.Id, RoleName.student))
                    {
                        userManager.AddToRole(existingUser.Id, RoleName.student);
                    }

                    TempData["Feedback"] = model.Name + " med e-post " + model.Email + " har lagts till";
                    return RedirectToAction("Create");
                }
                // If I get a conflict with data already in DB I trigger an error and the following method save it in ModelState
                // AddErrors(result);
                var resultModified = new IdentityResult(errorsInSwedish);
                AddErrors(resultModified);
            }

            // Model state is invalid: I need to fill the list of courses again and post it
            model.Courses = allCourses;

            // If we got this far, something failed, redisplay form
            return View("Create", model);
        }



        public ActionResult Students(int id)
        {
            var course = db.Courses.Where(c => c.Id == id).FirstOrDefault();
            var students = course.Students.OrderBy(s => s.Name).ToList();

            return PartialView("_StudentsPartial", students);
        }

        private List<ApplicationUser> AllStudents()
        {
            var role = db.Roles.SingleOrDefault(m => m.Name == RoleName.student);
            var students = db.Users.Where(u => u.Roles.Any(r => r.RoleId == role.Id)).ToList();

            return students;
        }

        public ActionResult Details(string email)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var student = userManager.FindByEmail(email);
            var studentViewModel = new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                CourseId = (int)student.CourseId,
                CourseName = student.Course.Name
            };

            return View(studentViewModel);
        }
        
        
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Edit(string email)
        {

            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var student = userManager.FindByEmail(email);
            var studentViewModel = new StudentViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                CourseId = (int)student.CourseId,
                Courses = CoursesOrderedByName()
            };

            return View(studentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Edit(StudentViewModel studentViewModel)
        {

            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (ModelState.IsValid)
            {
                //Check that email is not used by another user
                var student = userManager.FindByEmail(studentViewModel.Email);
                if (student != null && studentViewModel.Id != student.Id)
                {
                    ModelState.AddModelError("Email", "E-post adressen används redan");
                    studentViewModel.Courses = CoursesOrderedByName();
                    return View(studentViewModel);
                }

                // Get the existing student from the db
                var studentToUpdate = userManager.FindById(studentViewModel.Id);

                // Update it with the values from the view model
                studentToUpdate.Name = studentViewModel.Name;
                studentToUpdate.Email = studentViewModel.Email;
                studentToUpdate.CourseId = studentViewModel.CourseId;

                // Apply the changes if any to the db
                userManager.Update(studentToUpdate);

                //Get updated student from database
                var updatedStudent = userManager.FindById(studentViewModel.Id);
                var updatedStudentViewModel = new StudentViewModel
                {
                    Name = updatedStudent.Name,
                    Email = updatedStudent.Email,
                    CourseId = (int)updatedStudent.CourseId,
                    CourseName = updatedStudent.Course.FullCourseName
                };

                return View("Details", updatedStudentViewModel);
            }
            studentViewModel.Courses = CoursesOrderedByName();
            return View(studentViewModel);

        }

        public List<Course> CoursesOrderedByName()
        {
            return db.Courses.OrderBy(c => c.Name).ToList();
        }


        //}
        //public ActionResult DeleteUser(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var user = db.Users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(db.Users.Find(id));
        //}

        //public async Task<ActionResult> UserDeleteConfirmed(string id)
        //{
        //    var user = await UserManager.FindByIdAsync(id);

        //    var result = await UserManager.DeleteAsync(user);
        //    if (result.Succeeded)
        //    {
        //        TempData["UserDeleted"] = "User Successfully Deleted";
        //        return RedirectToAction("ManageEditors");
        //    }
        //    else
        //    {
        //        TempData["UserDeleted"] = "Error Deleting User";
        //        return RedirectToAction("ManageEditors");
        //    }
        //}
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}