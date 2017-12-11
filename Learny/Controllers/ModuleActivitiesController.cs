using Learny.Models;
using Learny.Settings;
using Learny.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Learny.Controllers
{
    [Authorize]
    public class ModuleActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ModuleActivities
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Index()
        {
            var activities = db.Activities.Include(m => m.ActivityType);
            return View(activities.ToList());
        }

        public ActionResult Activities(int id, bool linkToEditInCreateView = false)
        {
            var modulesActivities = db.Activities.Where(m => m.CourseModuleId == id).OrderBy(m => m.StartDate).ToList();
            var activities = new List<ModuleActivityViewModel>();
            foreach (var activity in modulesActivities)
            {
                activities.Add(new ModuleActivityViewModel(activity)
                {
                    Edit = linkToEditInCreateView
                });
            }

            return PartialView("_ActivitiesPartial", activities);
        }

        // GET: ModuleActivities/Details/5
        [Authorize(Roles = RoleName.teacher + "," + RoleName.student)]
        public ActionResult Details(int? id)
        {

            //Student may not view activity from other courses
            if (User.IsInRole(RoleName.student))
            {
                ApplicationUser currentUser = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                ModuleActivity userActivity = db.Activities.Where(a => a.Id == id).FirstOrDefault();
                CourseModule userModule = db.Modules.Where(m => m.Id == userActivity.CourseModuleId && m.CourseId == currentUser.CourseId).FirstOrDefault();

                if (userModule == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }


            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ModuleActivity moduleActivity = db.Activities.Find(id);
            if (moduleActivity == null)
            {
                return RedirectToAction("Index", "Home");
            }
            CourseModule module = db.Modules.Find(moduleActivity.CourseModuleId);
            if (module == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Course course = db.Courses.Find(module.CourseId);
            if (course == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var activity = new ModuleActivityViewModel
            {
                Id = moduleActivity.Id,
                Name = moduleActivity.Name,
                Description = moduleActivity.Description,
                StartDate = moduleActivity.StartDate,
                EndDate = moduleActivity.EndDate,
                CourseModuleId = moduleActivity.CourseModuleId,
                ActivityTypeName = moduleActivity.ActivityType.Name,
                ModuleName = module.Name,
                FullCourseName = course.Name,
                CourseId = course.Id

            };
            return View(activity);
        }

        #region Create Activity

        // GET: ModuleActivities/Create
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Create(int id)
        {
            var module = db.Modules.Where(m => m.Id == id).FirstOrDefault();
            if (module == null) return RedirectToAction("Index", "Home");

            var course = db.Courses.Where(c => c.Id == module.CourseId).FirstOrDefault();
            if (course == null) return RedirectToAction("Index", "Home");

            //Find previous activity's end date 
            DateTime startDate;
            var lastActivity = db.Activities.Where(m => m.CourseModuleId == id).OrderByDescending(m => m.EndDate).FirstOrDefault();
            if (lastActivity != null)
            {
                startDate = lastActivity.EndDate.AddDays(1);
            }
            else
            {
                //No previous activity exist, use modules start date 
                startDate = module.StartDate;
            }

            var activityViewModel = new ModuleActivityCreateViewModel
            {
                ModuleName = module.Name,
                CourseModuleId = id,
                FullCourseName = course.Name,
                CourseId = course.Id,
                StartDate = startDate,
                EndDate = startDate,
                ActivityTypes = db.ActivityTypes.ToList()

            };

            return View("Manage",activityViewModel);
        }

        // POST: ModuleActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleName.teacher)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseModuleId,ActivityTypeId,FullCourseName,CourseModuleId,ModuleName")] ModuleActivityCreateViewModel activityViewModel)
        {
            if (ModelState.IsValid)
            {
                var activity = new ModuleActivity
                {
                    Name = activityViewModel.Name,
                    Description = activityViewModel.Description,
                    StartDate = activityViewModel.StartDate,
                    EndDate = activityViewModel.EndDate,
                    CourseModuleId = activityViewModel.CourseModuleId,
                    ActivityTypeId = activityViewModel.ActivityTypeId,
                };

                var createdActivity = db.Activities.Add(activity);
                db.SaveChanges();

                TempData["FeedbackMessage"] = "Aktiviteten har lagts till";
                TempData["FeedbackData"] = activityViewModel;

                var newViewModel = new ModuleActivityCreateViewModel
                {
                    Edit = activityViewModel.Edit,
                    ListEdit = activityViewModel.ListEdit,
                    CourseId = activityViewModel.CourseId,
                    FullCourseName = activityViewModel.FullCourseName,
                    ModuleName = activityViewModel.ModuleName,
                    CourseModuleId = activityViewModel.CourseModuleId,
                    ActivityTypes = db.ActivityTypes.ToList()
            };

                return View("Manage", newViewModel);
                //return RedirectToAction("Details", "CourseModules",new { id = activityViewModel.CourseModuleId });
            }

            activityViewModel.ActivityTypes = db.ActivityTypes.ToList();
            return View("Manage", activityViewModel);
        }

        #endregion

        // Egidio: below is Edit for Activities

        // GET: ModuleActivities/Edit/5
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Edit(int? id, bool listEdit = false)
        {
            if (id == null) return RedirectToAction("Index", "Home");
            
            ModuleActivity moduleActivity = db.Activities.Find(id);
            if (moduleActivity == null) return RedirectToAction("Index", "Home");

            var activityViewModel = new ModuleActivityCreateViewModel(moduleActivity)
            {
                ActivityTypes = db.ActivityTypes.ToList(),
                Edit = true,
                ListEdit = listEdit
            };
            
            return View("Manage",activityViewModel);
        }

        // POST: ModuleActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleName.teacher)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,ActivityTypeId,CourseModuleId,ModuleName,FullCourseName,Edit,ListEdit")] ModuleActivityCreateViewModel activityViewModel)
        {
            if (ModelState.IsValid)
            {
                var activity = new ModuleActivity
                {
                    Id = activityViewModel.Id,
                    Name = activityViewModel.Name,
                    Description = activityViewModel.Description,
                    StartDate = activityViewModel.StartDate,
                    EndDate = activityViewModel.EndDate,
                    CourseModuleId = activityViewModel.CourseModuleId,
                    ActivityTypeId = activityViewModel.ActivityTypeId,
                };

                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();

                TempData["FeedbackMessage"] = "Modulen har ändrats";
                TempData["FeedbackData"] = activityViewModel;
            }

            if(activityViewModel.Edit == true && activityViewModel.ListEdit != true)
            {
                return RedirectToAction("Details", new { id = activityViewModel.Id });// View("Manage", activityViewModel);
            }
            activityViewModel.Edit = false;
            return RedirectToAction("Create", new { id = activityViewModel.CourseModuleId });// View("Manage", activityViewModel);
        }


        // GET: ModuleActivities/Delete/5
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ModuleActivity moduleActivity = db.Activities.Find(id);
            if (moduleActivity == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(moduleActivity);
        }

        // POST: ModuleActivities/Delete/5
        [Authorize(Roles = RoleName.teacher)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModuleActivity moduleActivity = db.Activities.Find(id);
            int? entityId = moduleActivity.CourseModuleId;
            bool allowDelete = true;
            if (moduleActivity.Documents.Count > 0)
            {
                allowDelete = false;
            }
            if (allowDelete)
            {
                db.Activities.Remove(moduleActivity);
                db.SaveChanges();
            }
            return RedirectToAction("Details", "CourseModules", new { id = entityId });
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
