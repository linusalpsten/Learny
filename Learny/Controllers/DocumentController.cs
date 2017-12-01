using Learny.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Learny.Controllers
{
    public class DocumentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Document
        public FileResult Download(int id)
        {
            var document = db.Documents.FirstOrDefault(d => d.Id == id);
            byte[] documentBytes = System.IO.File.ReadAllBytes(document.Path);
            return File(documentBytes, document.ContentType, document.DisplayName);
        }
    }
}