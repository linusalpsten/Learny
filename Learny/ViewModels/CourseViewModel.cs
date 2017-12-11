using Foolproof;
using Learny.DataAnnotations;
using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.ViewModels
{
    public class CourseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kursnamn är obligatoriskt.")]
        [Display(Name = "Kursnamn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kurskod är obligatoriskt.")]
        [Display(Name = "Kurskod")]
        public string CourseCode { get; set; }

        [Display(Name = "Kurskod och kursnamn")]
        public string FullCourseName { get; set; }

        [Required(ErrorMessage = "Startdatum är obligatoriskt.")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode=true)]
        [Display(Name = "Start")]
        [DateTimeToSqlDateTime]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Slutdatum är obligatoriskt.")]
        [GreaterThanOrEqualTo("StartDate", ErrorMessage = "Slutdatum får inte vara mindre än startdatum")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Slut")]
        [DateTimeToSqlDateTime]
        public DateTime EndDate { get; set; }

        [Display(Name = "Modul")]
        public ICollection<CourseModule> Modules { get; set; }
    }
}