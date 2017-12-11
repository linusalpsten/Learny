﻿using Learny.Models;
using Learny.Settings;
using Learny.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Learny.Controllers
{
    [Authorize]
    public class CourseModulesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CourseModules
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Index()
        {
            return View(db.Modules.ToList());
        }

        public ActionResult Modules(int id, bool linkToEditInCreateView=false)
        {
            var courseModules = db.Modules.Where(m => m.CourseId == id).OrderBy(m => m.StartDate).ToList();
            var modules = new List<ModuleViewModel>();
            foreach (var module in courseModules)
            {
                modules.Add(new ModuleViewModel(module)
                {
                    Edit = linkToEditInCreateView
                });
            }

            return PartialView("_ModulesPartial", modules);
        }

        // GET: CourseModules/Details/5
        [Authorize(Roles = RoleName.teacher + "," + RoleName.student)]
        public ActionResult Details(int? id)
        {
            //Student may not view module from other courses
            if (User.IsInRole(RoleName.student))
            {
                ApplicationUser currentUser = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                Course userCourse = db.Courses.Find(currentUser.CourseId);
                CourseModule userModule = db.Modules.Where(m => m.CourseId == userCourse.Id && m.Id == id).FirstOrDefault();
                if (userModule == null)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            CourseModule courseModule = db.Modules.Find(id);
            if (courseModule == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var course = db.Courses.Find(courseModule.CourseId);
            if (course == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var module = new ModuleViewModel(courseModule);
            return View(module);
        }

        // GET: 
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Create(int id)
        {
            var course = db.Courses.Where(c => c.Id == id).FirstOrDefault();
            if (course == null) return RedirectToAction("Index", "Home");

            //Find last modules end date 
            DateTime startDate;
            var lastModule = db.Modules.Where(m => m.CourseId == course.Id).OrderByDescending(m => m.EndDate).FirstOrDefault();
            if (lastModule != null)
            {
                startDate = lastModule.EndDate.AddDays(1);
            }
            else
            {
                //No previous module exist, use course start date 
                startDate = course.StartDate;
            }
            
            var viewModel = new ModuleViewModel
            {
                FullCourseName = course.FullCourseName,
                CourseId = id,
                StartDate = startDate,
                EndDate = startDate
            };

            return View("Manage", viewModel);
        }

        // POST: 
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleName.teacher)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId, FullCourseName, Edit, ListEdit")] ModuleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var courseModule = new CourseModule
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    CourseId = viewModel.CourseId
                };

                var createdModule = db.Modules.Add(courseModule);
                db.SaveChanges();
                    
                TempData["FeedbackMessage"] = "Modulen har lagts till";
                TempData["FeedbackData"] = viewModel;

                var newViewModel = new ModuleViewModel
                {
                    Edit = viewModel.Edit,
                    ListEdit = viewModel.ListEdit,
                    CourseId = viewModel.CourseId,
                    FullCourseName = viewModel.FullCourseName
                };

                return View("Manage", newViewModel);
            }

            return View("Manage", viewModel);
        }


        // Egidio: below is Edit for Modules

        // GET: CourseModules/Edit/5
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Edit(int? id, bool listEdit=false )
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            CourseModule courseModule = db.Modules.Find(id);
            if (courseModule == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var moduleView = new ModuleViewModel(courseModule)
            {
                Edit = true,
                ListEdit = listEdit
            };
            return View("Manage", moduleView);
        }


        // POST: CourseModules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleName.teacher)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,StartDate,EndDate,CourseId,FullCourseName,Edit,ListEdit")] ModuleViewModel moduleView)
        {
            if (ModelState.IsValid)
            {
                var courseModule = new CourseModule
                {
                    Id = moduleView.Id,
                    Name = moduleView.Name,
                    Description = moduleView.Description,
                    StartDate = moduleView.StartDate,
                    EndDate = moduleView.EndDate,
                    CourseId = moduleView.CourseId
                };
                db.Entry(courseModule).State = EntityState.Modified;
                db.SaveChanges();

                TempData["FeedbackMessage"] = "Modulen har ändrats";
                TempData["FeedbackData"] = moduleView;
            }

            return View("Manage", moduleView);
        }

        // GET: CourseModules/Delete/5
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            CourseModule courseModule = db.Modules.Find(id);
            if (courseModule == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(courseModule);
        }

        // POST: CourseModules/Delete/5
        [Authorize(Roles = RoleName.teacher)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseModule courseModule = db.Modules.Find(id);
            db.Modules.Remove(courseModule);
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
