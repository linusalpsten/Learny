using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.Models
{
    public class CourseModule
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Modul")]
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

        public int CourseId { get; set; }

        [Display(Name = "Kurs")]
        public virtual Course Course { get; set; }

        public virtual ICollection<ModuleActivity> Activities { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}