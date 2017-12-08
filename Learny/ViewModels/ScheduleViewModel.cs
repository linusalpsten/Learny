using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Learny.SharedClasses;
using System.ComponentModel.DataAnnotations;

namespace Learny.ViewModels
{
    public class ScheduleViewModel
    {
        public int CourseId { get; set; }

        [Display(Name = "Kurskod")]
        public string CourseCode { get; set; }

        [Display(Name = "Namn")]
        public string CourseName { get; set; }

        public string FullCourseName { get { return string.Format("{0} {1}", CourseCode, CourseName); } }

        public ICollection<OneScheduleEntry> ScheduleEntries { get; set; } 
    }
}