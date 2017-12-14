using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kursnamn")]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Kurskod")]
        public string CourseCode { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Startdatum")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Slutdatum")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public string FullCourseName => (CourseCode + " - " + Name).Trim();

        public virtual ICollection<ApplicationUser> Students { get; set; }

        public virtual ICollection<CourseModule> Modules { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}