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

        [Required(ErrorMessage = "Namn är obligatoriskt.")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kurskod är obligatoriskt.")]
        [Display(Name = "Kurskod")]
        public string CourseCode { get; set; }        

        [Required(ErrorMessage = "Startdatum är obligatoriskt.")]
        [Display(Name = "Start")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Slutdatum är obligatoriskt.")]
        [Display(Name = "Slut")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Moduler")]
        public ICollection<CourseModule> Modules { get; set; }
    }
}