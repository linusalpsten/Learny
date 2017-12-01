using Learny.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        public bool Upload(HttpPostedFileBase document)
        {
            if (document != null && document.ContentLength > 0)
            {
                try
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
                        FileName = documentName
                    };
                    db.Documents.Add(file);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                
            }
            return false;
        }

        public bool Upload(HttpPostedFileBase documentFile, Document document)
        {
            if (documentFile != null && documentFile.ContentLength > 0 && document != null)
            {
                try
                {
                    var now = DateTime.Now;
                    var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), GetHashString(now.ToString()));
                    documentFile.SaveAs(path);
                    document.Path = path;
                    document.ContentType = documentFile.ContentType;
                    document.TimeStamp = now;
                    document.FileName = documentFile.FileName;

                    db.Documents.Add(document);
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
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