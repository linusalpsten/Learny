﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.Models
{
    public class ModuleActivity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public int ActivityTypeId { get; set; }

        public int CourseModuleId { get; set; }


        public virtual ActivityType ActivityType { get; set; }

        [Display(Name = "Modul")]
        public virtual CourseModule Module { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}