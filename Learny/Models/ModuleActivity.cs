using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.Models
{
    public class ModuleActivity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start Datum")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Slut Datum")]
        public DateTime EndDate { get; set; }

        public int CourseModuleId { get; set; }

        public int ActivityTypeId { get; set; }

        [Display(Name = "Modul")]
        public virtual CourseModule CourseModule { get; set; }

        [Display(Name = "Aktivitets typ")]
        public virtual ActivityType ActivityType { get; set; }

        [Display(Name = "Dokument")]
        public virtual ICollection<Document> Documents { get; set; }
    }
}