using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class DocumentViewModel
    {
        public int? CourseId { get; set; }

        public int? CourseModuleId { get; set; }

        public int? ModuleActivityId { get; set; }

        [DisplayName("Namn")]
        public string Name { get; set; }

        [DisplayName("Beskrivning")]
        public string Description { get; set; }

        [DisplayName("Dokument")]
        public HttpPostedFileBase Document { get; set; }

        public string UploadTo { get; set; }
    }
}