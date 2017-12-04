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
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-Sv");
            //Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Ogiltigt inloggnings försök.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

       
        //
        // GET: /Account/CreateTeacher
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult CreateTeacher()
        {
            var viewModel = new TeacherViewModel { };
            return View(viewModel);
        }

        //
        // POST: /Account/CreateTeacher
        [HttpPost]
        [Authorize(Roles = RoleName.teacher)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTeacher(TeacherViewModel model)
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

                var result = UserManager.Create(user, model.Password);

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
                    var existingUser = UserManager.FindByName(model.Email);
                    if (!UserManager.IsInRole(existingUser.Id, RoleName.teacher))
                    {
                        UserManager.AddToRole(existingUser.Id, RoleName.teacher);
                    }


                    TempData["Feedback"] = "Läraren: " + model.Name + " med e-posten: " + model.Email + " har lagts till";
                    return RedirectToAction("CreateTeacher", "Account");
                }
                // Add swedish error message
                var resultModified = new IdentityResult(errorsInSwedish);
                AddErrors(resultModified);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ListTeachers()
        {
            List<ApplicationUser> students = AllStudents();

            List<ApplicationUser> allUsers = db.Users.ToList();

            var allTeachers = allUsers.Except(students).OrderBy(t => t.Name).ToList();

            List<TeacherViewModel> teachers = new List<TeacherViewModel>();

            foreach (var teacher in allTeachers)
            {
                teachers.Add(new TeacherViewModel{
                    Name = teacher.Name,
                    Email = teacher.Email
                });
            }

            return PartialView("_TeachersPartial", teachers);
        }


        #region Student
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult CreateStudentFromNavBar()
        {
            return RedirectToAction("CreateStudent");
        }


        // Student CREATE
        // GET: /Account/Register
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult CreateStudent(int? id)
        {
            if (id == null)
            {
                var allCourses = db.Courses.ToList();
                var viewModel = new StudentViewModel
                {
                    Courses = allCourses,
                    CourseSelected = false
                };

                return View(viewModel);
            }

            var course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            var viewModelSelectedCourse = new StudentViewModel
            {
                AttendingCourse = course.FullCourseName,
                CourseId = course.Id,
                CourseSelected = true
            };

            return View(viewModelSelectedCourse);

        }

        //
        // POST: /Account/CreateStudent (former Register)
        [HttpPost]
        [Authorize(Roles = RoleName.teacher)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateStudent(StudentViewModel model)
        {
            var allCourses = db.Courses.ToList();
            if (ModelState.IsValid)
            {
                // Check if email is already used by other users
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "En användare med den e-post adressen finns redan");
                    model.Courses = allCourses;
                    return View("CreateStudent", model);
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

                var result = UserManager.Create(user, model.Password);

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
                    //SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    var existingUser = UserManager.FindByName(model.Email);
                    if (!UserManager.IsInRole(existingUser.Id, RoleName.student))
                    {
                        UserManager.AddToRole(existingUser.Id, RoleName.student);
                    }

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //  return RedirectToAction("Index", "Home");

                    TempData["Feedback"] = "Elev: " + model.Name + " med e-posten: " + model.Email + " har lagts till";
                    return RedirectToAction("CreateStudent", "Account");
                }
                // If I get a conflict with data already in DB I trigger an error and the following method save it in ModelState
                // AddErrors(result);
                var resultModified = new IdentityResult(errorsInSwedish);
                AddErrors(resultModified);
            }

            // Model state is invalid: I need to feel the list of courses again and post it
            //model = new StudentVM { Courses = allCourses };
            model.Courses = allCourses;

            // If we got this far, something failed, redisplay form
            return View("CreateStudent", model);
        }



        public ActionResult Students(int id)
        {
            var course = db.Courses.Where(c => c.Id == id).FirstOrDefault();
            var students = course.Students.OrderBy(s => s.Name).ToList();

            return PartialView("_StudentsPartial", students);
        }


        public ActionResult ListStudents()
        {
            List<ApplicationUser> students = AllStudents();

            return View(students.Distinct().OrderBy(s => s.Name));
        }

        private List<ApplicationUser> AllStudents()
        {
            var students = new List<ApplicationUser>();
            foreach (var course in db.Courses)
            {
                students.AddRange(course.Students.ToList());
            }

            return students;
        }

        public ActionResult StudentDetails(string email)
        {
            var student = UserManager.FindByEmail(email);

            return View(student);
        }

        #endregion



        //





        [Authorize(Roles = RoleName.teacher)]
        public ActionResult EditStudent(string email)
        {

            var student = UserManager.FindByEmail(email);

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditStudent(StudentViewModel student)
        {
            if (student == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the existing student from the db
            var studentToUpdate = UserManager.FindById(student.Id);


            // Update it with the values from the view model
            studentToUpdate.Name = student.Name;
            studentToUpdate.Email = student.Email;

            // Apply the changes if any to the db
            UserManager.Update(studentToUpdate);

            //Get updated student from database
            var updatedStudent = UserManager.FindById(student.Id);

            return View("StudentDetails", updatedStudent);

        }




        public ActionResult ListUsers()
        {
            return View(db.Users.ToList());
        }

        public ActionResult EditUser(string email)
        {

            ApplicationUser appUser = new ApplicationUser();
            appUser = UserManager.FindByEmail(email);
            UserEdit user = new UserEdit();
            user.Name = appUser.Name;
            user.Email = appUser.Email;

            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(UserEdit model)
        {


            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(store);
            var currentUser = manager.FindByEmail(model.Email);
            currentUser.Name = model.Name;
            currentUser.Email = model.Email;
            await manager.UpdateAsync(currentUser);
            var ctx = store.Context;
            ctx.SaveChanges();
            TempData["msg"] = "Profile Changes Saved !";
            return RedirectToAction("ListUser");
        }
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(db.Users.Find(id));
        }

        public async Task<ActionResult> UserDeleteConfirmed(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            var result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["UserDeleted"] = "User Successfully Deleted";
                return RedirectToAction("ManageEditors");
            }
            else
            {
                TempData["UserDeleted"] = "Error Deleting User";
                return RedirectToAction("ManageEditors");
            }
        }




        //



        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }

    public class UserEdit
    {
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }


    }

}