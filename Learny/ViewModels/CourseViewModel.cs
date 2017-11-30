using Foolproof;
using Learny.DataAnnotations;
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

        [Display(Name = "Kursnamn och kurskod")]
        public string FullCourseName { get; set; }

        [Required(ErrorMessage = "Startdatum är obligatoriskt.")]
        [Display(Name = "Start")]
        [DateTimeToSqlDateTime]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Slutdatum är obligatoriskt.")]
        [GreaterThanOrEqualTo("StartDate", ErrorMessage = "Slutdatum får inte vara mindre än startdatum")]
        [Display(Name = "Slut")]
        [DateTimeToSqlDateTime]
        public DateTime EndDate { get; set; }

        [Display(Name = "Moduler")]
        public ICollection<CourseModule> Modules { get; set; }
    }
}