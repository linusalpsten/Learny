using Foolproof;
using Learny.DataAnnotations;
using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.ViewModels
{
    public class ModuleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Namn är obligatoriskt")]
        [Display(Name = "Modul")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Startdatum är obligatoriskt")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Startdatum")]
        [DateTimeToSqlDateTime]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Slutdatum är obligatoriskt")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Slutdatum")]
        [DateTimeToSqlDateTime]
        [GreaterThanOrEqualTo("StartDate", ErrorMessage = "Slutdatum får inte vara mindre än startdatum")]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        [Display(Name = "Kurs")]
        public string FullCourseName { get; set; }

        public virtual ICollection<ModuleActivity> Activities { get; set; }

        public bool Edit { get; set; }
        public bool ListEdit { get; set; }
    }
}