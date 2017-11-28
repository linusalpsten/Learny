using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Learny.ViewModels
{
    public class StudentCourseModuleViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        public virtual ICollection<ModuleActivity> Activities { get; set; }

    }
}