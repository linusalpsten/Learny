using Learny.Settings;
using Learny.SharedClasses;
using Learny.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace Learny.Models
{
    [Authorize]
    public class CoursesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = RoleName.teacher + "," + RoleName.student)]
        public ActionResult ShowSchedule(int? id)
        {
            if (id == null) return RedirectToAction("Index", "Home");

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

            var scheduleViewModel = new ScheduleViewModel();

            scheduleViewModel.CourseId = course.Id;
            scheduleViewModel.CourseName = course.Name;
            scheduleViewModel.CourseCode = course.CourseCode;

            var scheduleEntries = new List<OneScheduleEntry>();

            var activities = db.Activities.Where(a => a.Module.CourseId == course.Id).OrderBy(a => a.StartDate).ThenBy(a => a.EndDate).ToList();

            if (activities != null && activities.Count > 0)
            {
                DateTime startDate = activities.Min(a => a.StartDate);
                DateTime endDate = activities.Max(a => a.EndDate);

                TimeSpan dateDiff = endDate - startDate;
                int schemaDays = (int)dateDiff.TotalDays;
                DateTime currentDate;

                for (int daycounter = 0; daycounter <= schemaDays; daycounter++)
                {
                    currentDate = startDate.AddDays(daycounter);
                    // For each date check if any activity is ACTIVE.
                    // If so, then save it in onecourseEntry otherwise skip it
                    var activeActivities = activities.Where(a => a.StartDate <= currentDate && a.EndDate >= currentDate).ToList().OrderBy(a => a.Module.Id).ToList();

                    var oneScheduleEntry = new OneScheduleEntry();
                    oneScheduleEntry.CurrentDate = currentDate;
                    oneScheduleEntry.Activities = activeActivities;

                    scheduleEntries.Add(oneScheduleEntry);
                }
            }

            scheduleViewModel.ScheduleEntries = scheduleEntries;
            return View(scheduleViewModel);
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

                var changedCourse = db.Courses.Find(courseView.Id);

                TempData["FeedbackMessage"] = "Kursen har ändrats";
                TempData["FeedbackData"] = changedCourse;

                //return RedirectToAction("Details", new { id = courseView.Id });
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
