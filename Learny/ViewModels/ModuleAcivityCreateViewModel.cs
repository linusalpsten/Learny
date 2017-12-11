
using Foolproof;
using Learny.DataAnnotations;
using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;


namespace Learny.ViewModels
{
    public class ModuleActivityCreateViewModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage ="Aktivitetsnamn är obligatoriskt")]
        [Display(Name="Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Startdatum är obligatoriskt")]
        [DateTimeToSqlDateTime]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Slutdatum är obligatoriskt")]
        [GreaterThanOrEqualTo("StartDate", ErrorMessage = "Slutdatum får inte vara mindre än startdatum")]
        [DateTimeToSqlDateTime]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Modul")]
        public string ModuleName { get; set; }

        [Display(Name = "Kurs")]
        public string CourseName { get; set; }

        public int CourseModuleId { get; set; }

        public int CourseId { get; set; }

        [Required(ErrorMessage = "Aktivitetstyp är obligatorisk")]
        public int ActivityTypeId { get; set; }

        [Display(Name = "Aktivitetstyp")]
        public List<ActivityType> ActivityTypes { get; set; }

        public bool Edit { get; set; }
        public bool ListEdit { get; set; }

    }
}