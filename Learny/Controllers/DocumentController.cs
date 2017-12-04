using Learny.Models;
using Learny.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Learny.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var documents = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Documents.ToList();
            return View(documents);
        }

        [HttpGet]
        public ActionResult UploadDocument(int? courseId = null, int? moduleId = null, int? activityId = null)
        {
            var viewModel = new DocumentViewModel
            {
                CourseId = courseId,
                CourseModuleId = moduleId,
                ModuleActivityId = activityId,
            };
            if (courseId != null)
            {
                viewModel.UploadTo = " to " + db.Courses.FirstOrDefault(c => c.Id == courseId).FullCourseName;
            }
            else if (moduleId != null)
            {
                viewModel.UploadTo = " to " + db.Modules.FirstOrDefault(m => m.Id == moduleId).Name;
            }
            else if (activityId != null)
            {
                viewModel.UploadTo = " to " + db.Activities.FirstOrDefault(a => a.Id == activityId).Name;
            }
            return View(viewModel);
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
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Document
        public FileResult Download(int id)
        {
            var document = db.Documents.FirstOrDefault(d => d.Id == id);
            byte[] documentBytes = System.IO.File.ReadAllBytes(document.Path);
            return File(documentBytes, document.ContentType, document.DisplayName);
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