using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class StudentModuleActivity
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

        public int CourseModuleId { get; set; }
        
        public int ActivityTypeId { get; set; }
        
    }
}