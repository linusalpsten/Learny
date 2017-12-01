using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.Models
{
    public class CourseModule
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        public virtual ICollection<ModuleActivity> Activities { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}