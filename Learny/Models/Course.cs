using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Kurs kod")]
        public string CourseCode { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Start Datum")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Slut Datum")]
        public DateTime EndDate { get; set; }

        public string FullCourseName => (CourseCode + " - " + Name).Trim();

        public virtual ICollection<ApplicationUser> Students { get; set; }

        public virtual ICollection<CourseModule> Modules { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}