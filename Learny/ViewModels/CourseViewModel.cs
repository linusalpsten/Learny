using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Kurs kod")]
        public string CourseCode { get; set; }        

        [Required]
        [Display(Name = "Start")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Slut")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Moduler")]
        public ICollection<CourseModule> Modules { get; set; }
    }
}