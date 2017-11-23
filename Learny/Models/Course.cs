using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string CourseCode { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public virtual ICollection<ApplicationUser> Students { get; set; }

        public virtual ICollection<CourseModule> Modules { get; set; }
    }
}