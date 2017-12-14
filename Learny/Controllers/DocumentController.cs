using Learny.Models;
using Learny.Settings;
using Learny.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Learny.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Documents(int? courseId = null, int? moduleId = null, int? activityId = null)
        {
            var documents = new List<Document>();
            if (courseId != null) documents = db.Documents.Where(d => d.CourseId == courseId).ToList();
            if (moduleId != null) documents = db.Documents.Where(d => d.CourseModuleId == moduleId).ToList();
            if (activityId != null) documents = db.Documents.Where(d => d.ModuleActivityId == activityId).ToList();

            return PartialView("_DocumentsPartial", documents);
        }

        // GET: Document/Delete/5
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(document);
        }

        // POST: Document/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.teacher)]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Documents.Find(id);
            string controller = null;
            int? entityId = null;
            if (document.ModuleActivityId != null)
            {
                controller = "ModuleActivities";
                entityId = document.ModuleActivityId;
            }
            else if (document.CourseModuleId != null)
            {
                controller = "CourseModules";
                entityId = document.CourseModuleId;
            }
            else if (document.CourseId != null)
            {
                controller = "Courses";
                entityId = document.CourseId;
            }

            System.IO.File.Delete(document.Path);
            db.Documents.Remove(document);
            db.SaveChanges();

            if (controller != null && entityId != null)
            {
                return RedirectToAction("Details", controller, new { id = entityId });
            }
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public ActionResult UploadDocument(int? courseId = null, int? moduleId = null, int? activityId = null)
        {
            var viewModel = new DocumentViewModel
            {
                CourseId = courseId,
                CourseModuleId = moduleId,
                ModuleActivityId = activityId,
                MaxFileSize = MaxFileSizeToUpload(),
                MaxFileSizeKB = MaxFileSizeKB()
            };
            if (courseId != null)
            {
                var course = db.Courses.FirstOrDefault(c => c.Id == courseId);
                if (course != null) { 
                    viewModel.UploadTo = " till kurs " + course.FullCourseName;
                    viewModel.UploadToId = course.Id;
                    viewModel.UploadToIdType = HomeController.IdType.Course;
                }
            }
            else if (moduleId != null)
            {
                var module = db.Modules.FirstOrDefault(m => m.Id == moduleId);
                if (module != null) { 
                    viewModel.UploadTo = " till modul " + module.Name;
                    viewModel.UploadToId = module.Id;
                    viewModel.UploadToIdType = HomeController.IdType.Module;
                }
            }
            else if (activityId != null)
            {
                var activity = db.Activities.FirstOrDefault(a => a.Id == activityId);
                if (activity != null) { 
                    viewModel.UploadTo = " till aktivitet " + activity.Name;
                    viewModel.UploadToId = activity.Id;
                    viewModel.UploadToIdType = HomeController.IdType.Activity;
                }
            }
            return View(viewModel);
        }

        private static string MaxFileSizeToUpload()
        {
            var maxFileSizeKB = ((HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime")).MaxRequestLength;
            string maxFileSize = maxFileSizeKB + " KB";
            if (maxFileSizeKB >= 1024) maxFileSize = (int)(maxFileSizeKB * 0.0009765625) + " MB";
            if (maxFileSizeKB >= 1048576) maxFileSize = (int)(maxFileSizeKB * 0.00000095367432) + " GB";
            return maxFileSize;
        }

        private static int MaxFileSizeKB()
        {
            var maxFileSizeKB = ((HttpRuntimeSection)ConfigurationManager.GetSection("system.web/httpRuntime")).MaxRequestLength;
            return maxFileSizeKB;
        }

        [HttpPost]
        public ActionResult UploadDocument(DocumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var now = DateTime.Now;
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), GetHashString(now.ToString()));
                if (model.Document == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var document = new Document
                {
                    CourseId = model.CourseId,
                    CourseModuleId = model.CourseModuleId,
                    ModuleActivityId = model.ModuleActivityId,
                    Name = model.Name,
                    Description = model.Description
                };
                Upload(model.Document, document);


                if (model.CourseId != null) return RedirectToAction("Details", "Courses", new { id= model.CourseId });
                if (model.CourseModuleId != null) return RedirectToAction("Details", "CourseModules", new { id = model.CourseModuleId });
                if (model.ModuleActivityId != null) return RedirectToAction("Details", "ModuleActivities", new { id = model.ModuleActivityId });

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Document
        public FileContentResult Download(int id)
        {
            var document = db.Documents.FirstOrDefault(d => d.Id == id);
            byte[] documentBytes = System.IO.File.ReadAllBytes(document.Path);
            return File(documentBytes, document.ContentType, document.DisplayName + document.Extension);
        }

        public FileContentResult OpenDocument(int id)
        {
            var document = db.Documents.FirstOrDefault(d => d.Id == id);
            byte[] documentBytes = System.IO.File.ReadAllBytes(document.Path);
            Response.AppendHeader("Content-Disposition", "inline; filename=" + document.FileName);
            return File(documentBytes, document.ContentType);
        }

        public void Upload(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0)
            {
                var documentName = document.FileName;
                var now = DateTime.Now;
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), GetHashString(now.ToString()));
                document.SaveAs(path);
                var file = new Document
                {
                    Path = path,
                    ContentType = document.ContentType,
                    TimeStamp = now,
                    FileName = documentName,
                    UserId = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id
                };
                db.Documents.Add(file);
                db.SaveChanges();
            }
        }

        public void Upload(HttpPostedFileBase documentFile, Document document)
        {
            if (documentFile != null && documentFile.ContentLength > 0 && document != null)
            {
                var now = DateTime.Now;
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), GetHashString(now.ToString()));
                documentFile.SaveAs(path);
                document.Path = path;
                document.ContentType = documentFile.ContentType;
                document.TimeStamp = now;
                document.FileName = documentFile.FileName;
                document.UserId = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id;

                db.Documents.Add(document);
                db.SaveChanges();
            }
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}