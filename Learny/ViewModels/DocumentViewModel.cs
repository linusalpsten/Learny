using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class DocumentViewModel
    {
        public int? CourseId { get; set; }

        public int? CourseModuleId { get; set; }

        public int? ModuleActivityId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public HttpPostedFileBase Document { get; set; }
    }
}