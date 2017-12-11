using Learny.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Learny.SharedClasses;
using Learny.ViewModels;

namespace Learny.Models
{
    [Authorize]
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        // The course id is passed to to this Action which act as a GET
        [Authorize(Roles = RoleName.teacher + "," + RoleName.student)]
        public ActionResult ShowSchedule(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Course course;
            if (User.IsInRole(RoleName.student))
            {
                ApplicationUser CurrentUser = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                course = db.Courses.Find(CurrentUser.CourseId);
            }
            else
            {
                course = db.Courses.Where(c => c.Id == id).FirstOrDefault();
            }
            var courseEntries = new List<OneScheduleEntry>();

            var ScheduleVM = new ScheduleViewModel();

            // Populate the VM with Course infos
            ScheduleVM.CourseId = course.Id;
            ScheduleVM.CourseName = course.Name;
            ScheduleVM.CourseCode = course.CourseCode;

            // All modules for THIS Course
            var courseModules = course.Modules.Where(m => m.Activities.Count != 0).OrderBy(m => m.StartDate).ToList();
            DateTime currentDate;
            foreach (var module in courseModules)
            {
                TimeSpan difference = module.EndDate - module.StartDate;
                int moduleDays = (int)difference.TotalDays;

                // All activities for THIS module
                var moduleActivities = module.Activities.ToList();

                // date counter
                for (int daycounter = 0; daycounter < moduleDays; daycounter++)
                {


                    // Update the date counter
                    currentDate = module.StartDate.AddDays(daycounter);



                    // For each date check if any activity is ACTIVE.
                    // If so, the save it in onecourseEntry otherwise skip it
                    List<ModuleActivity> activities = new List<ModuleActivity>();
                    foreach (var activity in moduleActivities)
                    {
                        var activityStart = activity.StartDate;
                        var activityEnd = activity.EndDate;
                        if (DateTime.Compare(currentDate, activityStart) >= 0 &&
                            DateTime.Compare(currentDate, activityEnd) <= 0)
                        {
                            // the current activity is ACTIVE in currentDate
                            // and can be stored in the "dayly schedule entry"
                            activities.Add(activity);
                        }
                    }
                    if (activities.Count != 0)
                    {
                        var oneCourseEntry = new OneScheduleEntry();
                        oneCourseEntry.ActivityNamesList = new List<string>();
                        oneCourseEntry.CurrentDate = currentDate;
                        oneCourseEntry.ModuleName = module.Name;
                        foreach (var activity in activities)
                        {
                            oneCourseEntry.ActivityNamesList.Add(activity.Name);
                        }
                        courseEntries.Add(oneCourseEntry);
                    }
                    // save in the final data structure to be shown on the view

                }
            }
            ScheduleVM.ScheduleEntries = courseEntries;
            return View(ScheduleVM);
        }


        // GET: Courses
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Index()
        {
            var courses = db.Courses.ToList();
            List<CourseDetailsViewModel> viewModels = new List<CourseDetailsViewModel>();
            foreach (var course in courses)
            {
                viewModels.Add(new CourseDetailsViewModel(course));
            }

            var sortedViewModel = viewModels.OrderBy(v => v.StartDate);
            return View(sortedViewModel);
        }


        // GET: Courses/Details/5
        [Authorize(Roles = RoleName.teacher + "," + RoleName.student)]
        public ActionResult Details(int? id)
        {

            if (User.IsInRole(RoleName.student))
            {
                ApplicationUser currentUser = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                id = currentUser.CourseId;
            }

            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Course course = db.Courses.Find(id);
            if (course == null) return RedirectToAction("Index", "Home");
            CourseDetailsViewModel viewModel = new CourseDetailsViewModel(course);
            return View(viewModel);
        }

        // GET: Courses/Create
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Create([Bind(Include = "Id,Name,CourseCode,Description,StartDate,EndDate")] CourseCreateViewModel courseView)
        {
            if (ModelState.IsValid)
            {
                if (db.Courses.Any(c => c.CourseCode == courseView.CourseCode))
                {
                    ModelState.AddModelError("CourseCode", "Kurskoden finns redan");
                    return View(courseView);
                }
                var course = new Course
                {
                    Id = courseView.Id,
                    Name = courseView.Name,
                    CourseCode = courseView.CourseCode,
                    Description = courseView.Description,
                    StartDate = courseView.StartDate,
                    EndDate = courseView.EndDate
                };

                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseView);
        }


        // Egidio: below is Edit for Courses

        // GET: Courses/Edit/5
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var courseView = new CourseCreateViewModel
            {
                Id = course.Id,
                Name = course.Name,
                FullCourseName = course.FullCourseName,
                CourseCode = course.CourseCode,
                Description = course.Description,
                StartDate = course.StartDate,
                EndDate = course.EndDate
            };
            return View(courseView);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Edit([Bind(Include = "Id,Name,CourseCode,Description,StartDate,EndDate")] CourseCreateViewModel courseView)
        {
            if (ModelState.IsValid)
            {
                // If I change the code of the current course (=the courseCode retrieved from the DB is different from the one I just filled in)
                // I need to check this code does not exist already in the DB!

                // OBS! If we use db.Courses.Find(courseViewId).CourseCode .... Entity Framework will retrieve a whole new object from DB
                // This will cause a conflict in the ' db.Entry(course).State = EntityState.Modified;' below.
                // Therefore I nned to use following alternatives:
                // 1. db.Courses.AsNoTracking().FirstOrDefault(c =>c.Id == courseView.Id)?.CourseCode 
                // 2. db.Courses.Where(c => c.Id == courseView.Id).Select (c => c.CourseCode)

                if (db.Courses.AsNoTracking().FirstOrDefault(c => c.Id == courseView.Id)?.CourseCode != courseView.CourseCode)
                {
                    if (db.Courses.Any(c => c.CourseCode == courseView.CourseCode && c.Id != courseView.Id))
                    {
                        // I found a course in the DB with exactly the SAME course code than I inserted.
                        // This is not allowed
                        ModelState.AddModelError("CourseCode", "Kurskoden finns redan");
                        return View(courseView);
                    }
                }

                //if (db.Courses.Any(c => c.CourseCode == courseView.CourseCode))
                //{
                //    ModelState.AddModelError("CourseCode", "Kurskoden finns redan");
                //    return View(courseView);
                //}

                var course = new Course
                {
                    Id = courseView.Id,
                    Name = courseView.Name,
                    CourseCode = courseView.CourseCode,
                    Description = courseView.Description,
                    StartDate = courseView.StartDate,
                    EndDate = courseView.EndDate
                };
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = courseView.Id });
            }
            return View(courseView);
        }




        // GET: Courses/Delete/5
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            bool allowDelete = true;
            if (course.Documents.Count > 0)
            {
                allowDelete = false;
            }
            else
            {
                foreach (var module in course.Modules)
                {
                    if (module.Documents.Count > 0)
                    {
                        allowDelete = false;
                        break;
                    }
                    foreach (var activity in module.Activities)
                    {
                        if (activity.Documents.Count > 0)
                        {
                            allowDelete = false;
                            break;
                        }
                    }
                    if (!allowDelete)
                    {
                        break;
                    }
                }
            }
            if (allowDelete)
            {
                db.Courses.Remove(course);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
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
