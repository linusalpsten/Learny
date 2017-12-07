using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class ModuleActivityViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
        
        [Display(Name = "Modul")]
        public string ModuleName { get; set; }

        public int CourseModuleId { get; set; }

        [Display(Name = "Kurs")]
        public string CourseName { get; set; }

        public int CourseId { get; set; }

        public string ActivityTypeName { get; set; }


    }
}