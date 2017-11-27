using Learny.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class CourseDetailsViewModel : CourseViewModel
    {
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Elever")]
        public ICollection<ApplicationUser> Students { get; set; }
    }
}