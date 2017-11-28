using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class StudentCourseViewModel
    {

        [Display(Name = "Namn")]
        public string Name { get; set; }
        
        [Display(Name = "Kurskod")]
        public string CourseCode { get; set; }
        
        [Display(Name = "Start")]
        public DateTime StartDate { get; set; }
                
        [Display(Name = "Slut")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Moduler")]
        public ICollection<CourseModule> Modules { get; set; }

    }
}