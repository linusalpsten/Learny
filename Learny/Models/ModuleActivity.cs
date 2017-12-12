using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.Models
{
    public class ModuleActivity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Aktivitet")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }

        public int ActivityTypeId { get; set; }

        public int CourseModuleId { get; set; }

        [Display(Name = "Modul")]
        public virtual CourseModule Module { get; set; }

        [Display(Name = "Aktivitetstyp")]
        public virtual ActivityType ActivityType { get; set; }

        [Display(Name = "Dokument")]
        public virtual ICollection<Document> Documents { get; set; }
    }
}