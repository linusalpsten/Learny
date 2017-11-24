using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Learny.ViewModels
{
    public class CourseCreateViewModel : CourseViewModel
    {
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

    }
}