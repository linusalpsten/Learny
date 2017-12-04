using Learny.Models;
using Learny.Settings;
using Learny.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Learny.Controllers
{
    [Authorize(Roles = RoleName.teacher)]
    public class ModuleActivitiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ModuleActivities
        public ActionResult Index()
        {
            var activities = db.Activities.Include(m => m.ActivityType);
            return View(activities.ToList());
        }

        // GET: ModuleActivities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleActivity moduleActivity = db.Activities.Find(id);
            if (moduleActivity == null)
            {
                return HttpNotFound();
            }
            var activity = new ModuleActivityViewModel
            {
                Id = moduleActivity.Id,
                Name = moduleActivity.Name,
                Description = moduleActivity.Description,
                StartDate = moduleActivity.StartDate,
                EndDate = moduleActivity.EndDate,
                CourseModuleId = moduleActivity.CourseModuleId,
                ActivityTypeName = moduleActivity.ActivityType.Name
            };
            return View(activity);
        }

#region Create Activity

        // GET: ModuleActivities/Create
        public ActionResult Create(int id)
        {

            //checked if module id exist
            if (!db.Modules.Any(m => m.Id == id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var currentDateTime = DateTime.Now;
            var today = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day);

            var activityViewModel = new ModuleActivityCreateViewModel
            {
                CourseModuleId = id,
                StartDate = today,
                EndDate = today,
                ActivityTypes = db.ActivityTypes.ToList()

            };

            return View(activityViewModel);
        }

        // POST: ModuleActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseModuleId,ActivityTypeId")] ModuleActivityCreateViewModel activityViewModel)
        {

            if (ModelState.IsValid)
            {
                if (activityViewModel.StartDate == DateTime.MinValue)
                {
                    ModelState.AddModelError("StartDate", "Startdatum måste vara större än 0");
                }
                if (activityViewModel.EndDate == DateTime.MinValue)
                {
                    ModelState.AddModelError("EndDate", "Slutdatum måste vara större än 0");
                }

                var activity = new ModuleActivity
                {
                    Name = activityViewModel.Name,
                    Description = activityViewModel.Description,
                    StartDate = activityViewModel.StartDate,
                    EndDate = activityViewModel.EndDate,
                    CourseModuleId = activityViewModel.CourseModuleId,
                    ActivityTypeId = activityViewModel.ActivityTypeId,
                };

                db.Activities.Add(activity);
                db.SaveChanges();
                return RedirectToAction("Details", "CourseModules",new { id = activityViewModel.CourseModuleId });
            }

            activityViewModel.ActivityTypes = db.ActivityTypes.ToList();
            return View(activityViewModel);
        }

#endregion

        // Egidio: below is Edit for Activities

        // GET: ModuleActivities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleActivity moduleActivity = db.Activities.Find(id);
            if (moduleActivity == null)
            {
                return HttpNotFound();
            }

            var activityViewModel = new ModuleActivityCreateViewModel
            {
                Id = moduleActivity.Id,
                Name = moduleActivity.Name,
                Description = moduleActivity.Description,
                StartDate = moduleActivity.StartDate,
                EndDate = moduleActivity.EndDate,
                CourseModuleId = moduleActivity.CourseModuleId,
                ActivityTypes = db.ActivityTypes.ToList(),
                ActivityTypeId = moduleActivity.ActivityTypeId
            };

         //   ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", moduleActivity.ActivityTypeId);

            return View(activityViewModel);
        }

        // POST: ModuleActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseModuleId,ActivityTypeId")] ModuleActivityCreateViewModel activityViewModel)
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
                return RedirectToAction("Details", new { id = activityViewModel.Id });
            }

            activityViewModel.ActivityTypes = db.ActivityTypes.ToList();

            // ViewBag.ActivityTypeId = new SelectList(db.ActivityTypes, "Id", "Name", activityViewModel.ActivityTypeId);

            return View(activityViewModel);
        }


        // GET: ModuleActivities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ModuleActivity moduleActivity = db.Activities.Find(id);
            if (moduleActivity == null)
            {
                return HttpNotFound();
            }
            return View(moduleActivity);
        }

        // POST: ModuleActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ModuleActivity moduleActivity = db.Activities.Find(id);
            db.Activities.Remove(moduleActivity);
            db.SaveChanges();
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
